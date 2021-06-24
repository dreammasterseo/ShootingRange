using UnityEngine;
using UnityEngine.UI;

public class Aim : MonoBehaviour {

    public int points;
    public Text aimCount;
    public GameObject message;

	public void SubAimCount()
    {
        message.GetComponent<Text>().text = "+" + points;
        message.GetComponent<Animation>().Play();

        aimCount.text = (int.Parse(aimCount.text) - 1).ToString();
    }
}