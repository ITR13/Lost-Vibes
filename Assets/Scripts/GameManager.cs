using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LostVibes
{
    public partial class GameManager : MonoBehaviour
    {
        [SerializeField] public Camera camera;
        [SerializeField] public float fromHue, toHue;

        [SerializeField] private float boundVelocity;
        [SerializeField] private float boundRange;
        [SerializeField] private float falloffPerSecond;
        [SerializeField] private SpriteSlider spriteSlider;

        [SerializeField] private Button[] buttons;
        [SerializeField] private Button silenceButton;

        private float _chillTensity = 0.5f;
        private float _ctVelocity = 0.0f;

        private bool _ctIsBound = false;
        private float _ctBoundPosition = 0.5f;

        private float _ctBias = 0f;
        private float _vibeCooldown = 0f;



        private static readonly Vibe[] _vibes = new[]
        {
            new Vibe
            {
                Name = "Chill",
                Velocity = -0.2f,
                Bias = 0f,
                Cooldown = 1f,
                Resistance = 0.6f,
            },
            new Vibe
            {
                Name = "Melancholy",
                Velocity = 0f,
                Bias = -0.05f,
                Cooldown = 1f,
                Resistance = 0.3f,
            },
            new Vibe
            {
                Name = "Angy",
                Velocity = 0.2f,
                Bias = 0f,
                Cooldown = 1f,
                Resistance = 0.6f,
            },

            new Vibe
            {
                Name = "Mellow",
                Velocity = -0.1f,
                Bias = 0.1f,
                Cooldown = 1f,
                Resistance = 0.6f,
            },
            new Vibe
            {
                Name = "Passive Aggressive",
                Velocity = 0f,
                Bias = 0.02f,
                Cooldown = 1f,
                Resistance = 0.3f,
            },
            new Vibe
            {
                Name = "Emo",
                Velocity = 0.1f,
                Bias = -0.1f,
                Cooldown = 1f,
                Resistance = 0.6f,
            },
        };

        private Dictionary<string, Vibe> _vibeDict = _vibes.ToDictionary(vibe => vibe.Name);


        private Dictionary<string, float> _vibeInfluence = new Dictionary<string, float>();

        private void Start()
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                var j = i;
                buttons[i].GetComponentInChildren<Text>().text = _vibes[i].Name;
                buttons[i].onClick.AddListener(() => ExecuteVibe(_vibes[j]));
            }

            silenceButton.onClick.AddListener(() =>
                {
                    _vibeCooldown = 1;
                    _ctBias *= 0.7f;
                    foreach (var key in _vibeInfluence.Keys.ToArray())
                    {
                        _vibeInfluence[key] = Mathf.MoveTowards(_vibeInfluence[key], 1, 0.3f);
                    }
                }
            );
        }

        private void Update()
        {
            camera.backgroundColor = Color.HSVToRGB(
                Mathf.Lerp(fromHue, toHue, _chillTensity),
                1,
                0.5f
            );

            spriteSlider.SetMarkerPosition(_chillTensity);
            foreach (var button in buttons)
            {
                button.interactable = _vibeCooldown <= 0;
            }

            silenceButton.interactable = _vibeCooldown <= 0;

            AfterUpdate();
        }

        private void FixedUpdate()
        {
            foreach (var key in _vibeInfluence.Keys.ToArray())
            {
                _vibeInfluence[key] = Mathf.MoveTowards(_vibeInfluence[key], 1, Time.fixedDeltaTime / (10 * _vibeDict[key].Cooldown));
            }


            _vibeCooldown -= Time.fixedDeltaTime;
            if (Mathf.Abs(_ctVelocity) <= boundVelocity)
            {
                if (!_ctIsBound)
                {
                    _ctIsBound = true;
                    _ctBoundPosition = _chillTensity;

                    spriteSlider.SetBoundPosition(_ctBoundPosition, boundRange);
                    spriteSlider.SetBoundsVisible(true);
                }

                BoundUpdate();
            }
            else
            {
                if (_ctIsBound)
                {
                    _ctIsBound = false;
                    spriteSlider.SetBoundsVisible(false);
                }

                FreeUpdate();
            }
        }

        private void FreeUpdate()
        {
            _chillTensity += Time.fixedDeltaTime * _ctVelocity;
            _ctVelocity = Mathf.MoveTowards(_ctVelocity, 0, falloffPerSecond * Time.deltaTime);
        }

        private void BoundUpdate()
        {
            _ctVelocity = Mathf.Sign(_ctVelocity) * boundVelocity;
            _chillTensity += Time.fixedDeltaTime * _ctVelocity;

            var min = _ctBoundPosition - boundRange;
            var max = _ctBoundPosition + boundRange;

            if (_chillTensity < min)
            {
                _chillTensity = min * 2 - _chillTensity;
                _ctVelocity = Mathf.Abs(_ctVelocity);
            }
            else if (_chillTensity > max)
            {
                _chillTensity = max * 2 - _chillTensity;
                _ctVelocity = -Mathf.Abs(_ctVelocity);
            }
        }

        partial void AfterUpdate();

        private void ExecuteVibe(Vibe vibe)
        {
            if (_vibeCooldown > 0)
            {
                Debug.LogWarning("Tried to execute vibe before cooldown finished");
                return;
            }

            if (!_vibeInfluence.ContainsKey(vibe.Name))
            {
                _vibeInfluence.Add(vibe.Name, 1);
            }

            var influence = _vibeInfluence[vibe.Name] * _vibeInfluence[vibe.Name] * _vibeInfluence[vibe.Name];

            _vibeCooldown = vibe.Cooldown;
            _ctVelocity += (vibe.Velocity + Mathf.Abs(vibe.Velocity) * _ctBias * Mathf.Abs(_ctBias)) * influence;
            _ctBias += vibe.Bias * influence;

            _vibeInfluence[vibe.Name] = Mathf.MoveTowards(_vibeInfluence[vibe.Name], 0, vibe.Resistance);
        }
    }
}