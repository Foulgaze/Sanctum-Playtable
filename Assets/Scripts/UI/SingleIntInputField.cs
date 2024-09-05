using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SingleIntInputField : MonoBehaviour
{
	public TextMeshProUGUI name;
	public TMP_InputField input;
	public Button submitBtn;
	public TextMeshProUGUI submitBtnText;
	[SerializeField] private Button closeBtn;

	public void Setup(string boxName, string submitBtnName)
	{
		name.text = boxName;
		submitBtnText.text = submitBtnName;
	}
	void Start()
	{
		closeBtn.onClick.AddListener(() => Destroy(this));
		submitBtn.onClick.AddListener(() => Destroy(this));
	}
}