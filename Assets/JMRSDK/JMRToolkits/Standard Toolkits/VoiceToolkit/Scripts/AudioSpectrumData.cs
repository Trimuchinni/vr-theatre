using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioSpectrumData : MonoBehaviour
{
	AudioSource _audioSource;
	public static float[] _samples = new float[512];
	public static float[] _freqBand = new float[8];
	public static float[] _bandBuffer = new float[8];
	float[] _bufferDecrease = new float[8];
	
	float[] _freqBandHighest=new float[8];
	public static float[] _audioBand= new float[8];
	public static float[] _audioBandBuffer=new float[8];
	public static float _Amplitude, _AmplitudeBuffer;
	float _AmplitudeHighest;
	public float _CurrentAmplitude;
	
    // Start is called before the first frame update
    void Start()
    {
        _audioSource=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
		MakeFrequencyBands();
		BandBuffer();
		CreateAudioBands();
		getAmplitude();
		_CurrentAmplitude = _Amplitude;

	}
	
	void getAmplitude()
	{
		float _CurrentAmplitude=0;
		float _CurrentAmplitudeBuffer=0;
		for(int i=0;i<8;i++)
		{
			_CurrentAmplitude+=_audioBand[i];
			_CurrentAmplitudeBuffer+=_audioBandBuffer[i];
		}
		if(_CurrentAmplitude> _AmplitudeHighest){
			_AmplitudeHighest=_CurrentAmplitude;
	}
	_Amplitude = _CurrentAmplitude / _AmplitudeHighest;
	_AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;	
	}
	void CreateAudioBands()
	{
		for(int i =0; i<8 ;i++)
		{
			if(_freqBand[i]> _freqBandHighest[i]){
				_freqBandHighest[i] = _freqBand[i];
			}
			_audioBand[i]=(_freqBand[i]/_freqBandHighest[i]); 
			_audioBandBuffer[i]=(_bandBuffer[i]/_freqBandHighest[i]);
		}
	}
	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples,0, FFTWindow.Blackman);
	
}
void BandBuffer()
{
	for (int g=0;g<8;++g)
	{
		if(_freqBand[g]>_bandBuffer[g])
		{
			_bandBuffer[g]=_freqBand[g];
			_bufferDecrease[g]=0.0005f;
		}

}
}
         void MakeFrequencyBands()
           {
			   int count=0;
			   for(int i=0;i<8;i++)
			   {
				   float avarage=0;
				   int sampleCount = (int)Mathf.Pow( 2,i) * 2;
				   if(i==7)
				   {
					   sampleCount+=2;
				   }
				   for(int j=0; j < sampleCount;j++)
				   {
					   avarage+=_samples[count]*(count+1);
					   count++;
				   }
				   avarage/= count;
				   _freqBand[i]=avarage*10;
			   }
		   }
		   
}