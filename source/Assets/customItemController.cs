﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System;
using System.IO;

public class customItemController : rewardOrPunishmentController {

	public string imagePath = "";
	public int material, kinematicStyle;
	
	// Use this for initialization
	void Start () {
		//disappearAfterTouching = false;
	}
	
	public bool initialize(string fileName, string name, Vector2 position, float rotation, float endorphins, float mass, int material, int kinematicStyle)
	{
		Texture2D tex = null;
		byte[] fileData;
		if (File.Exists(fileName))     {
			fileData = File.ReadAllBytes(fileName);
			tex = new Texture2D(5, 5);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
			
			Sprite newSprite = new Sprite();
			newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 50f);
			//should it be a worldObject, endorphinthing or what?
			transform.position = new Vector3(position.x, position.y, 0);
			transform.rotation = new Quaternion(0, 0, rotation, 0);
			this.endorphins = endorphins; // ADDED BY AE
			if (endorphins==0)
				disappearAfterTouching = false;
			else
				disappearAfterTouching = true;

			SpriteRenderer sr = GetComponent<SpriteRenderer>();
			sr.sprite = newSprite;
			BoxCollider2D b = GetComponent<BoxCollider2D>();
			b.offset = newSprite.bounds.center;
			b.size = newSprite.bounds.size;
			//fill out parameters
			objectName = name;
			GetComponent<Rigidbody2D>().mass = mass;
			//set friction
			b.sharedMaterial = (PhysicsMaterial2D)Resources.Load("PhysicsMaterials2D/pm" + material);
			//kinematic
			//kinematic: 0,1,6
			GetComponent<Rigidbody2D>().isKinematic = (kinematicStyle==0 || kinematicStyle==1 || kinematicStyle==6);
			//backgrounded: 0,2,4
			int backgroundLayer = LayerMask.NameToLayer("Nonreactive");
			if (kinematicStyle == 0 || kinematicStyle == 2 || kinematicStyle == 4) {
				gameObject.layer = backgroundLayer;
				gameObject.GetComponent<SpriteRenderer> ().sortingLayerName = "background";
			} else if (kinematicStyle == 6) {  //pseudo-backgrounded: 6
				gameObject.layer = LayerMask.NameToLayer ("VisibleButNonreactive");
				gameObject.GetComponent<SpriteRenderer> ().sortingLayerName = "background";
			}
			
			//fixed angle: 0,1 (implied), 2, 3
			GetComponent<Rigidbody2D>().fixedAngle = (kinematicStyle==2 || kinematicStyle==3);
			this.material = material;
			this.kinematicStyle = kinematicStyle;
			this.imagePath = fileName;

			saveVals ();

			return true;
		}
		else
			return false;
	}
	
	public override void saveVals()
	{
		//save all triggerbox options
		Dictionary<string,System.Object> newValuesToSave = new Dictionary<string,System.Object>();
		newValuesToSave.Add("imagePath", imagePath);
		newValuesToSave.Add("material", material);
		newValuesToSave.Add("endorphins", endorphins);
		newValuesToSave.Add("kinematicStyle", kinematicStyle);
		newValuesToSave.Add("objectName", objectName);
		newValuesToSave.Add ("positionX", transform.position.x);
		newValuesToSave.Add ("positionY", transform.position.y);
		newValuesToSave.Add ("rotation", transform.rotation.y); 
		newValuesToSave.Add ("mass", GetComponent<Rigidbody2D> ().mass);
		//newValuesToSave.Add("name", this.name);

		valuesToSave = newValuesToSave;
	}
	
	public override void loadVals()
	{
		//triggerOnBodyEnter = (bool)valuesToSave["triggerOnBodyEnter"];		
		
		this.imagePath = (string)valuesToSave["imagePath"];
		this.material = (int)valuesToSave["material"];
		this.kinematicStyle = (int)valuesToSave["kinematicStyle"];
		this.endorphins = (float)valuesToSave["endorphins"];
		this.objectName = (string)valuesToSave["objectName"];

		//this.name = (string)valuesToSave["name"];
		//transform.localScale = new Vector3((float)valuesToSave["scalex"], (float)valuesToSave["scaley"]);
		initialize(this.imagePath, this.objectName, new Vector2((float)valuesToSave["positionX"],(float)valuesToSave["positionY"]),
			(float)valuesToSave["rotation"], this.endorphins, (float)valuesToSave["mass"], this.material, this.kinematicStyle);
	}
	
}
