using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Obstacles;
using JumpMaster.UI.Data;

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

    public class MissileWarning : MonoBehaviour
    {
        public event MissileWarningEventHandler OnWarningEnded;

        private MissileWarningSO _info;

        private MissileWarningData _data;

        private float _flashInterval;

        private RawImage _image;

        private RectTransform _rect;

        private bool _ended = false;

        private void Start()
        {
            if (_info == null)
            {
                Debug.LogError("Please use the Generate function for creating missile warnings!");
                enabled = false;
            }
        }

        public static MissileWarning Generate(MissileWarningSO info, MissileWarningData data)
        {
            GameObject game_object = Instantiate(info.Prefab);

            game_object.SetActive(false);

            MissileWarning generated = game_object.GetComponent<MissileWarning>();

            generated.Initialize(info, data);

            return generated;
        }

        private void Initialize(MissileWarningSO info, MissileWarningData data)
        {
            _info = info;
            _data = data;

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

            StartCoroutine(Countdown());
            StartCoroutine(Action());
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(_data.Duration);

            _ended = true;
        }

        private IEnumerator Action()
        {
            _image.enabled = true;

            yield return new WaitForSeconds(_flashInterval);

            if (_ended)
            {
                if (OnWarningEnded != null)
                    OnWarningEnded();
                Destroy(gameObject);
            }

            _image.enabled = false;

            yield return new WaitForSeconds(_flashInterval);

            yield return Action();
        }
    }
}