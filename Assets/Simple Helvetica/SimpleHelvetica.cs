//Simple Helvetica. Copyright © 2012. Studio Pepwuper, Inc. http://www.pepwuper.com/
//email: info@pepwuper.com
//version 1.0

using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleHelvetica : MonoBehaviour {
    public float CircleRadius = 100.0f; //spacing between the characters
    [HideInInspector]
	public string Text = "SIMPLE HELVETICA\n \nby Studio Pepwuper";
	[HideInInspector]
	public float CharacterSpacing = 4f; //spacing between the characters
   [HideInInspector]
	public float LineSpacing = 22f;
	[HideInInspector]
	public float SpaceWidth = 8f; //how wide should the "space" character be?
	
	private float CharXLocation = 0f;
	private float CharYLocation = 0f;
	
	private Vector3 ObjScale; //the scale of the parent object
	
	void Awake(){
		
#if !UNITY_3_4 && !UNITY_3_5
		//disable _Alphabets and all children under it to remove them from being seen.
		transform.Find("_Alphabets").gameObject.SetActive(false);
		
#else
		//disable _Alphabets and all children under it to remove them from being seen.
		transform.Find("_Alphabets").gameObject.SetActiveRecursively(false);
#endif
		
	}
	
	//Reset is called when the reset button is clicked on the inspector
	void Reset(){
		GenerateText();	
	}
	
	
	//Generate New 3D Text
	public void GenerateText() {
		
		//Debug.Log ("GenerateText Called");
		
		ResetText(); //reset before generating new text
		
		//check all letters
		for (int ctr = 0; ctr <= Text.Length - 1; ctr++ ){			
			
			//Debug.Log ("Text Length" + Text.Length);
			//Debug.Log ("ctr"+ctr);
			
			//dealing with linebreaks "\n"
			if ( Text[ctr].ToString().ToCharArray()[0] == "\n"[0] ){
				//Debug.Log ("\\n detected");
				CharXLocation = 0;
				CharYLocation -= LineSpacing;
				continue;
			}
				
			string childObjectName = Text[ctr].ToString();
			
						
			if (childObjectName!=" "){
				
				GameObject LetterToShow;
								
				if (childObjectName =="/"){
					LetterToShow = transform.Find("_Alphabets/"+"slash").gameObject; //special case for "/" since it cannot be used for obj name in fbx					
				} else if (childObjectName=="."){
					LetterToShow = transform.Find("_Alphabets/"+"period").gameObject; //special case for "." - naming issue	
				} else {
					LetterToShow = transform.Find("_Alphabets/"+childObjectName).gameObject;
				}
				
				//Debug.Log(LetterToShow);
				
				AddLetter(LetterToShow);
				
				//find the width of the letter used
				Mesh mesh = LetterToShow.GetComponent<MeshFilter>().sharedMesh;
				Bounds bounds = mesh.bounds;
				CharXLocation += bounds.size.x;
				//Debug.Log (bounds.size.x*ObjScale.x);
			}
			else {
				CharXLocation += SpaceWidth;
			}
		}

#if !UNITY_3_4 && !UNITY_3_5
		//disable child objects inside _Alphabets
		transform.Find("_Alphabets").gameObject.SetActive(false);
#else
		//disable child objects inside _Alphabets
		transform.Find("_Alphabets").gameObject.SetActiveRecursively(false);
#endif
		
	}
	

	void AddLetter(GameObject LetterObject){
   		
		GameObject NewLetter = Instantiate(LetterObject, transform.position, transform.rotation) as GameObject;
		NewLetter.transform.parent=transform; //setting parent relationship
		
		//rename instantiated object
		NewLetter.name = LetterObject.name;
		
		//scale accoring to parent obj scale
		float newScaleX = NewLetter.transform.localScale.x*ObjScale.x; 
		float newScaleY = NewLetter.transform.localScale.y*ObjScale.y; 
		float newScaleZ = NewLetter.transform.localScale.z*ObjScale.z; 
		
		Vector3 newScaleAll = new Vector3(newScaleX, newScaleY, newScaleZ);
		NewLetter.transform.localScale = newScaleAll;
		//------------------------------------
		
		//dealing with characters with a line down on the left (kerning, especially for use with multiple lines)
		if (CharXLocation  < 0.001f)
			if (NewLetter.name == "B" || 
				NewLetter.name == "D" || 
				NewLetter.name == "E" || 
				NewLetter.name == "F" || 
				NewLetter.name == "H" || 
				NewLetter.name == "I" || 
				NewLetter.name == "K" || 
				NewLetter.name == "L" || 
				NewLetter.name == "M" || 
				NewLetter.name == "N" || 
				NewLetter.name == "P" || 
				NewLetter.name == "R" || 
				NewLetter.name == "U" || 
				NewLetter.name == "b" || 
				NewLetter.name == "h" || 
				NewLetter.name == "i" ||
				NewLetter.name == "k" ||
				NewLetter.name == "l" ||
				NewLetter.name == "m" ||
				NewLetter.name == "n" ||
				NewLetter.name == "p" ||
				NewLetter.name == "r" ||
				NewLetter.name == "u" || 
				NewLetter.name == "|" || 
				NewLetter.name == "[" || 
				NewLetter.name == "!")
				CharXLocation += 2;

        //position the new char
        float angle = Mathf.Deg2Rad * CharXLocation;
        Debug.Log("Circle Radius: " + this.CircleRadius);
        float xPosOnCircle = this.CircleRadius * Mathf.Cos(angle);
        float zPosOnCircle = this.CircleRadius * Mathf.Sin(angle);
        NewLetter.transform.localPosition = new Vector3(xPosOnCircle,CharYLocation,zPosOnCircle);
        NewLetter.transform.localRotation = Quaternion.LookRotation(-1f*Vector3.Normalize(NewLetter.transform.localPosition));
        CharXLocation += CharacterSpacing; //add a small space between words
	}
	
	
	void ResetText(){
		
		//reset scale
		//transform.localScale = new Vector3(1,1,1);
		
		//get object scale
		ObjScale = transform.localScale;
		
		//reset position
		CharXLocation = 0f;
		CharYLocation = 0f;
		
		//remove all previous created letters
		Transform[] previousLetters;
		previousLetters = GetComponentsInChildren<Transform>();
		foreach(Transform childTransform in previousLetters){
			if (childTransform.name != "_Alphabets" && childTransform.name != transform.name && childTransform.parent.name != "_Alphabets"){
				//Debug.Log("previous letter: "+childTransform.name);
				DestroyImmediate(childTransform.gameObject);	
			}
			
		}
		
	}
	
	public void ApplyMeshRenderer(){
		
		MeshRenderer selfMeshRenderer=GetComponent<MeshRenderer>();
		bool selfMesherRendererCastShadows = selfMeshRenderer.castShadows;
		bool selfMesherRendererReceiveShadows = selfMeshRenderer.receiveShadows;
		Material[] selfMesherRendererSharedMaterials = selfMeshRenderer.sharedMaterials;
		bool selfMesherRendererUseLightProbes = selfMeshRenderer.useLightProbes;
		Transform selfMesherRendererLightProbeAnchor = selfMeshRenderer.probeAnchor;
			
		Debug.Log ("Apply MeshRenderer");
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			MeshRenderer thisMeshRenderer = child.gameObject.GetComponent<MeshRenderer>();
			Debug.Log (selfMeshRenderer);
			if (thisMeshRenderer!=null){
				thisMeshRenderer.castShadows = selfMesherRendererCastShadows;
				thisMeshRenderer.receiveShadows = selfMesherRendererReceiveShadows;
				thisMeshRenderer.sharedMaterials = selfMesherRendererSharedMaterials;
				thisMeshRenderer.useLightProbes = selfMesherRendererUseLightProbes;
				thisMeshRenderer.probeAnchor = selfMesherRendererLightProbeAnchor;
			}
		}
		
		foreach (Transform child in transform){
			MeshRenderer thisMeshRenderer = child.gameObject.GetComponent<MeshRenderer>();
			if (thisMeshRenderer!=null){
				thisMeshRenderer.castShadows = selfMesherRendererCastShadows;
				thisMeshRenderer.receiveShadows = selfMesherRendererReceiveShadows;
				thisMeshRenderer.sharedMaterials = selfMesherRendererSharedMaterials;
				thisMeshRenderer.useLightProbes = selfMesherRendererUseLightProbes;
				thisMeshRenderer.probeAnchor = selfMesherRendererLightProbeAnchor;
			}
		}
		
		
	}
	
	public void DisableSelf(){
		
		enabled=false;
		//Debug.Log ("enabled? "+enabled);
		
	}
	
	public void EnableSelf(){
		
		enabled=true;
		//Debug.Log ("enabled? "+enabled);
		
	}
	
}
