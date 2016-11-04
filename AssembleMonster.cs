using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEditorInternal;
using UnityEditor.VersionControl;

public class AssembleMonster : MonoBehaviour 
{
	public GameObject Monster;
	public Animator MonsterAnimator;
	public AnimationClip MonsterClip;
	public List<string> ClipNames = new List<string> ();
	public AnimationClip[] Try;
	public ModelImporter MonstersClip;
	public List<AnimationClip> clipList = new List <AnimationClip>();


	// Use this for initialization
	void Awake () 
	{
		Monster = GameObject.FindGameObjectWithTag ("Monster");
		MonstersClip = (ModelImporter)AssetImporter.GetAtPath ("Assets/Resources/NPC's/Original/Aranhas/aranha_Boss");
	}

	void Start()
	{
		MonsterAnimator = Monster.GetComponent <Animator> ();
		MonsterAnimator.runtimeAnimatorController = Resources.Load ("Controllers/All") as RuntimeAnimatorController;

		foreach(AnimationClip clip in MonsterAnimator.runtimeAnimatorController.animationClips)
		{
			ClipNames.Add (clip.name);
		}
	}  

	void OnPostprocessModel(GameObject go)
	{        
		AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);

		foreach (AnimationClip clip in clips)
		{
			AnimationClip newClip = new AnimationClip();
			clipList.Add(newClip);
		}
	}  
}
