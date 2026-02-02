using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ Is Editor_ New", menuName = "Behaviors/Conditions/Application/Is Editor")]
public class IsEditorConditionBehavior : ConditionBehavior
{
	protected override bool Evaluate()
	{
#if UNITY_EDITOR
		return true;
#else
		return false;
#endif
	}
}