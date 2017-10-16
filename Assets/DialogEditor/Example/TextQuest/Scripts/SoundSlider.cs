using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour {

    private void OnEnable()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("soundVolume");
        GetComponent<Slider>().onValueChanged.AddListener(VolumeChanged);
    }

    private void VolumeChanged(float arg0)
    {
        QuestBookLibrary.Instance.SetSound(arg0);
    }
}
