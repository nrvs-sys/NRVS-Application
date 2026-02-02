using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentBuildOnly : MonoBehaviour
{
	private void OnEnable() => gameObject.SetActive(Debug.isDebugBuild);
}