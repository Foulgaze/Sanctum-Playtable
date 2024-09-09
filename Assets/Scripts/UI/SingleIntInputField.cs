using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SingleIntInputField : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI name;
	[SerializeField] private TMP_InputField input;
	[SerializeField] private Button submitBtn;
	[SerializeField] private TextMeshProUGUI submitBtnText;
	[SerializeField] private Button closeBtn;

	public void Setup(string boxName, Action<string> submitAction, string submitBtnName)
	{
		name.text = boxName;
		submitBtn.onClick.AddListener(() => {submitAction(input.text);});
		submitBtnText.text = submitBtnName;
	}
	void Start()
	{
		closeBtn.onClick.AddListener(() => Destroy(transform.gameObject));
		submitBtn.onClick.AddListener(() => Destroy(transform.gameObject));

	}
}