using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour {

    public static Text goldText;

    void Awake() {
        goldText = GetComponentInChildren<Text>();
    }
    void Start()
    {
        Goldupdate();
    }

    void Update()
    {
        Goldupdate();
    }

    public static void Goldupdate() {
        goldText.text = Data_Player.Gold.ToString();
    }
}
