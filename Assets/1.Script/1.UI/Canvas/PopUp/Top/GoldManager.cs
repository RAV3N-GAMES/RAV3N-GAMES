using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour {
    public Text goldText;

    void Start()
    {
        Goldupdate();
    }

    void Update()
    {
        Goldupdate();
    }

    public void Goldupdate() {
        goldText.text = Data_Player.Gold.ToString();
    }
}
