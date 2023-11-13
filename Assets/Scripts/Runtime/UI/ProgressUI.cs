using Runtime.DataContainers;
using Runtime.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Runtime.UI
{
    [RequireComponent(typeof(Station))]
    public class ProgressUI : MonoBehaviour
    {
        [SerializeField]
        private Image _progressImageBackground;
        [SerializeField]
        private Image _progressImage;
        [SerializeField]
        private Image _alert;
        [SerializeField]
        private Canvas _progressCanvas;
        private Station _station;
        private bool _isProcessing;
        List<IEnumerator> _coroutineList = new List<IEnumerator>();
        private Coroutine _currentCoroutine;
        private bool _isPaused;

        [SerializeField]
        private ParticleSystem _fireSystem;

        private void Awake()
        {
            _fireSystem.Stop();
        }

        private void Start()
        {
            _progressCanvas.worldCamera = Camera.main;

            if (_alert)
                _alert.gameObject.SetActive(false);
            _station = GetComponent<Station>();
            _station.OnStationProcessStarted += OnProcessStart;
            _station.OnStationProcessCancelled += ProcessCancelled;
            _station.OnStationProcessPause += Pause;
            _station.OnStationProcessResume += Resume;
            _station.OnStationClicked += AdjustFire;
            _isProcessing = false;
        }

        private void AdjustFire()
        {
            if(_isPaused)
            {
                _fireSystem.transform.localScale -= Vector3.one * 0.15f;
            }
        }
        private void Pause()
        {
            _fireSystem.transform.localScale = Vector3.one * 0.8f;
            _fireSystem.Play();
            _isPaused = true;
            _alert.gameObject.SetActive(true);
            _progressImageBackground.gameObject.SetActive(false);
            _progressImage.color = Color.red;
        }
        private void Resume()
        {
            _fireSystem.Stop();
            _isPaused = false;
            _alert.gameObject.SetActive(false);
            _progressImageBackground.gameObject.SetActive(true);
            _progressImage.color = Color.white;
        }

        private void ProcessCancelled()
        {
            if (!_isProcessing) return;
            
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
            _isProcessing = false;
        }

        private void OnProcessStart(float _processTime)
        {
            _currentCoroutine = StartCoroutine(RunProgressBar(_processTime));
        }

        private void Update()
        {
            if(_isProcessing)
            {
                if(!_progressCanvas.enabled)
                    _progressCanvas.enabled = true;

                _progressCanvas.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
            }
            else
            {
                if (_progressCanvas.enabled)
                    _progressCanvas.enabled = false;
            }
        }

        IEnumerator RunProgressBar(float _seconds)
        {
            _isProcessing = true;
            float t = 0;

            while(t < _seconds)
            {
                if (_isPaused)
                {
                    yield return null;
                    continue;
                }

                t += Time.deltaTime;
                _progressImage.fillAmount = t / _seconds;
                yield return null;
            }
            _isProcessing = false;
            _currentCoroutine = null;
        }

        private void OnDestroy()
        {
            _station.OnStationProcessStarted -= OnProcessStart;
            _station.OnStationProcessCancelled -= ProcessCancelled;
            _station.OnStationProcessPause -= Pause;
            _station.OnStationProcessResume -= Resume;
            _station.OnStationClicked -= AdjustFire;
        }

        public Station Station { get => _station; }
        public Image ProgressImage { get => _progressImage; }
    }
}
