using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVoiceVisualizer : MonoBehaviour
{
    public float _startSpeed, _startSimSpeed,_AlphaMultiFloat;
    public GameObject _psObj;
    public bool _useIntensity, _useSimSpeed;
    public bool _OnMicEnable;
    public float _intensity = 1.0f;
    public float _divider = 10.0f;
    
    ParticleSystem _ps;
    float lerpTime = 0;
    //public float _red,_green,_blue;
    // Start is called before the first frame update
    public float _startScale, _endScale, _lerpValue;
    
    void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        var _rotationBySpeed = _ps.rotationBySpeed;
        var _main = _ps.main;
        _rotationBySpeed.enabled = true;
       

    }

    // Update is called once per frame
    void Update()
    {
        
        //_test.transform.localScale = new Vector3(transform.localScale.x, (AudioSpectrumData._Amplitude)*10 + 100, transform.localScale.z);
        var _rotationBySpeed = _ps.rotationBySpeed;
        var _main = _ps.main;
        _AlphaMultiFloat = Mathf.RoundToInt(AudioSpectrumData._Amplitude*10);
        lerpTime += Time.deltaTime;
        if (_OnMicEnable)
        {
            if (_useIntensity)
            {
                _rotationBySpeed.z = ((AudioSpectrumData._Amplitude * _startSpeed) / _divider) * _intensity;
            }
            else
            {
                _rotationBySpeed.z = (AudioSpectrumData._Amplitude * _startSpeed) / 10;
            }
            if (_useSimSpeed)
            {
                _main.simulationSpeed = _startSimSpeed * AudioSpectrumData._Amplitude;
            }
            _lerpValue = Mathf.Lerp(_startScale, _endScale, lerpTime);
            this.transform.localScale = new Vector3(_lerpValue, _lerpValue, _lerpValue);
            
            if (_AlphaMultiFloat > 1)
            {
                GetComponent<Renderer>().sharedMaterial.SetFloat("_AlphaM", _AlphaMultiFloat);
            }
        }
        else
        {
            _lerpValue = Mathf.Lerp(_endScale, _startScale, lerpTime);
            this.transform.localScale = new Vector3(_lerpValue, _lerpValue, _lerpValue); ;
        }
       
        
    }
  

}
