using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisualization : MonoBehaviour
{
    AudioSource _audioSource;


    public static float[] _samples = new float[512];
    public static float[] _freqBand = new float[8];
    public static float[] _bandBuffer = new float[8];
    private float[] _bufferDecrease = new float[8];


    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];


    private float[] _prevFrameBandBuffer = new float[8];
    public float[] _changeInIntensity = new float[8];

    private float[] _pulseCooldown = new float[8];


    [SerializeField] private GameObject[] musicPulses;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float minSize = 0f;
    [SerializeField] private float maxSize = 1000f;
    [SerializeField] private float[] freqSizeRatio = new float[8];

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        ResetBandHighest();
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        ChangeInFrequency();
        CreateAudioBands();

        #region Music Pulses

        float highestIntensity = 0;
        int highestIndex = 0;
        for(int i = 0; i < 8;i++)
        {
            if (_pulseCooldown[i] > 0) 
            {
                _pulseCooldown[i] -= Time.deltaTime;
            }

            if (_pulseCooldown[i] < 0)
            {
                _pulseCooldown[i] = 0;
            }

            if (_changeInIntensity[i] > highestIntensity)
            {
                highestIntensity = _changeInIntensity[i];
                highestIndex = i;
            }

            if (_changeInIntensity[i] > 0.3f && _pulseCooldown[i] <= 0)
            {
             //   Debug.Log(i);
            }
        }

        for(int j = 0;j < musicPulses.Length;j++)
        {
            float freqRatio = (float)(1 - (j / 8f));
            Material mat = musicPulses[j].GetComponent<MeshRenderer>().material;            

            float scale = Mathf.Lerp(minSize, freqSizeRatio[j] * maxSize, _audioBandBuffer[j]);
            musicPulses[j].transform.localScale = new Vector3(scale, scale, scale);
            //mat.SetFloat("_Range", 1.5f / (freqRatio * 100 * _changeInIntensity[j]));
            mat.SetVector("_MainColor", colorGradient.Evaluate(freqRatio) * freqSizeRatio[j] * (_audioBandBuffer[j] * 5));
            mat.SetFloat("_Range", Mathf.Lerp(0.01f / freqSizeRatio[j], 0.1f, 1 - _audioBandBuffer[j]));
            mat.SetFloat("_Transparency", Mathf.Lerp(0, 200, 1 - _audioBandBuffer[j]));
            

            /*
            if (Mathf.Abs(highestIntensity - _changeInIntensity[j]) < 0.1f && highestIntensity > 0.2f && _pulseCooldown[j] <= 0){
                //GameObject newPulse = Instantiate(musicPulse, transform.position, Quaternion.identity);

                float freqRatio = (float)(1 - (j / 8f));
                Material mat = newPulse.GetComponent<MeshRenderer>().material;
                mat.SetVector("_MainColor", colorGradient.Evaluate(freqRatio) * 5);
                mat.SetFloat("_Range", 1.5f / (freqRatio * 100 * _changeInIntensity[j]));

                _pulseCooldown[j] = 0.1f;
            }
            */
        }

        #endregion

    }

    void CreateAudioBands()
    {
        for(int i = 0;i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }

            _audioBand[i] = _freqBand[i] / _freqBandHighest[i];
            _audioBandBuffer[i] = _bandBuffer[i] / _freqBandHighest[i];
        }
    }

    
    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }


    void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBand[i];
                _bufferDecrease[i] = 0.01f;
            }

            if (_freqBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f;
            }
        }

    }
    void MakeFrequencyBands()
    {
        /* 
         * 0-2   (86)
         * 1-4   (87 - 258)
         * 2-8   (259 - 602)
         * 3-16  (603 - 1290)
         * 4-32  (1291 - 2666)
         * 5-64  (2667 - 5418)
         * 6-128 (5419 - 10922)
         * 7-256 (10923 - 21930)
         * 
         */

        int count = 0;

        for (int i = 0;i < 8;i++)
        {

            float average = 0;

            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == 7)
            {
                sampleCount += 2;
            }//Ensure 512 samples

            for(int j = 0;j < sampleCount;j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;


            _freqBand[i] = average * 10;
        }
    }

    void ChangeInFrequency()
    {

        for(int i = 0; i < 8;i++)
        {
            _changeInIntensity[i] = _audioBandBuffer[i] - _prevFrameBandBuffer[i];
            _prevFrameBandBuffer[i] = _audioBandBuffer[i];
        }
    }

 
    public void ResetBandHighest()
    {
        for(int i = 0;i < 8;i++)
        {
            _freqBandHighest[i] = 0.5f;
        }
    }
}
