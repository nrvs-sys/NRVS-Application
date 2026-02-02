using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Condition_ Scene Open_ New", menuName = "Behaviors/Conditions/Application/Scene Open")]
public class SceneOpenConditionBehavior : ConditionBehavior
{
    [SerializeField]
    SceneReference scene;

    override protected bool Evaluate() =>
        SceneManager.GetSceneByName(scene.SceneName()).isLoaded;
}
