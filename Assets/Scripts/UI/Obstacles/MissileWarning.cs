using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Structure;
using JumpMaster.Obstacles;
using JumpMaster.UI.Data;
using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public struct MissileWarningData
    {
        public readonly Vector2 ScreenPosition;

        public readonly MissileDirection Direction;

        public readonly float Duration;

        public MissileWarningData(Vector2 screen_position, MissileDirection direction, float duration)
        {
            ScreenPosition = screen_position;

            Direction = direction;

            Duration = duration;
        }
    }

    public delegate void MissileWarningEventHandler();

    public class MissileWarning : InitializablePausable
    {
        public event MissileWarningEventHandler OnWarningEnded;

        private MissileWarningSO _info;

        private MissileWarningData _data;

        private float _flashInterval;

        private RawImage _image;

        private RectTransform _rect;

        private bool _ended = false;
        private float _countdownStartTime = 0f;

        private void Start()
        {
            if (_info == null)
            {
                Debug.LogError("Please use the Generate function for creating missile warnings!");
                enabled = false;
            }
        }

        private static MissileWarningSO s_info;
        private static MissileWarningData s_data;

        public static MissileWarning Generate(MissileWarningSO info, MissileWarningData data)
        {
            GameObject game_object = Instantiate(info.Prefab);

            game_object.SetActive(false);

            MissileWarning generated = game_object.GetComponent<MissileWarning>();

            s_info = info;
            s_data = data;

            generated.Initialize();
            generated.Initialized = true;

            return generated;
        }

        protected override void Initialize()
        {
            _info = s_info;
            _data = s_data;

            _ended = false;

            Vector3 margined_screen_position = _data.ScreenPosition;
            switch(_data.Direction)
            {
                case MissileDirection.UP:
                    margined_screen_position.y += _info.ScreenMargin;
                    break;

                case MissileDirection.DOWN:
                    margined_screen_position.y -= _info.ScreenMargin;
                    break;

                case MissileDirection.LEFT:
                    margined_screen_position.x -= _info.ScreenMargin;
                    break;

                case MissileDirection.RIGHT:
                    margined_screen_position.x += _info.ScreenMargin;
                    break;
            }

            _rect = GetComponent<RectTransform>();
            _rect.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>(), false);
            _rect.localScale = Vector3.one;
            _rect.anchoredPosition = margined_screen_position;

            _image = GetComponent<RawImage>();
            _image.enabled = false;

            _flashInterval = _info.FlashIntervalMS / 1000f;

            gameObject.SetActive(true);

            _countdownStartTime = Time.time;

            StartCoroutine(Action());
        }

        protected override void Pause()
        {
            
        }

        protected override void Unpause()
        {
            _countdownStartTime += LevelController.LastPauseDuration;
        }

        protected override void PlayerDeath()
        {
            Restart();
        }

        protected override void Restart()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            if (LevelController.Paused)
                return;

            if (_ended)
                return;

            if (Time.time - _countdownStartTime > _data.Duration)
                _ended = true;
        }

        private IEnumerator Action()
        {
            _image.enabled = true;

            yield return new WaitForSecondsPausable(_flashInterval);

            if (_ended)
            {
                if (OnWarningEnded != null)
                    OnWarningEnded();
                Destroy(gameObject);
            }

            _image.enabled = false;

            yield return new WaitForSecondsPausable(_flashInterval);

            yield return Action();
        }
    }
}