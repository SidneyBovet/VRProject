﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class PlanetController : MonoBehaviour {
	
	public GameObject sunPrefab;
	public GameObject planetPrefab;
	public GameObject starClusterPrefab;
	public GameObject asteroidBeltPrefab;
	
	public Material executable;
	public Material text;
	public Material image;
	public Material video;
	public Material music;
	public Material document;
	public Material model3D;
	public Material other;
	public GameObject textfileContentPrefab;
	
	private bool canSelect = true;
	private SolarSystem[] systems;
	private Vector3 posInit = new Vector3(1000f, 0f, 0f);
	private enum FileType {
		Executable,
		Text,
		Image,
		Video,
		Music,
		Document,
		Model3D,
		Other
	}
	
	void Start () {
		systems = new SolarSystem[2];
		systems[0] = new SolarSystem (this, new DirectoryInfo ("."), Vector3.zero);
	}

	void Update() {
		if (systems [0] != null && systems [1] != null && systems [0].origin.magnitude > 0.1f) {
			float speed = Mathf.Max(0.001f, 1-systems[0].origin.magnitude/posInit.magnitude);
			Vector3 move = Vector3.Lerp(Vector3.zero, systems [0].origin, speed*0.1f);
			systems [0].setOrigin (systems [0].origin - move);
			systems [1].setOrigin (systems [1].origin - move);
		} else {
			canSelect = true;
		}
	}

	public void Selection() {
		Collider selected = transform.Find ("/OVRPlayerController").GetComponent<UserController>().GetSelection();
		if (!canSelect || selected == null)
			return;

		if (selected.CompareTag ("File")) {
			canSelect = false;
			string[] extention = selected.transform.parent.name.Split (new Char[] {'.'});
			Debug.Log (extention [extention.Length - 1]);
			switch (extention [extention.Length - 1]) {
			case "txt":
				Debug.Log ("TextFile selected!");
				StartCoroutine (DisplayFile (FileType.Text, selected.transform.parent));
				break;
			default:
				Debug.Log ("Unknown filetype selected!");
				StartCoroutine (DisplayFile (FileType.Other, selected.transform.parent));
				break;
			}
		} else if (selected.CompareTag ("Folder")) {
			canSelect = false;
			if(systems[1] != null) {
				systems[1].removeAll();
				systems[1] = null;
			}
			changeSolarSystem(selected.gameObject, posInit);
		}
	}
	
	private bool changeSolarSystem(GameObject newSystem, Vector3 origin) {
		DirectoryInfo newDir = null;
		
		if(newSystem == systems[0].sun) {
			newDir = systems[0].dir.Parent;
		} else {
			for (int i=0; i<systems[0].startClusters.Length; i++) {
				if(newSystem == systems[0].startClusters[i]) {
					newDir = systems[0].dirs[i];
					break;
				}
			}
		}
		if(newDir == null) {
			return false;
		}

		systems [1] = systems [0];
		systems[0] = new SolarSystem (this, newDir, origin);
		return true;
	}
	
	IEnumerator DisplayFile(FileType type, Transform file) {
		switch (type) {
			case FileType.Text:
				for (int i = 0; i < 10; i++) {
					file.Translate(new Vector3(0f,0.2f,0f));
					yield return new WaitForSeconds(0.05f);
				}
				GameObject content = GameObject.Instantiate(
					textfileContentPrefab, 
					textfileContentPrefab.transform.position + new Vector3(0f,file.position.y + 0.5f,file.position.z),
					Quaternion.identity) as GameObject;
				content.GetComponent<TextMesh>().text = "TODO: get textfile content\nand use it to feed this\nGameObject, insering linefeeds\nif necessary.";
				break;
			case FileType.Other:
				for (int i = 0; i < 5; i++) {
					file.Translate(new Vector3(0f,0.1f,0f));
					yield return new WaitForSeconds(0.01f);
				}
				for (int i = 0; i < 5; i++) {
					file.Translate(new Vector3(0f,-0.1f,0f));
					yield return new WaitForSeconds(0.01f);
				}
				break;
		}
		canSelect = true;
	}
	
	
	
	private class SolarSystem {
		public DirectoryInfo dir;
		public DirectoryInfo[] dirs;
		public FileInfo[] files;
		
		public GameObject sun;
		public GameObject[] startClusters;
		public GameObject[] planets;
		
		public Vector3 origin;
		private PlanetController pc;
		
		public SolarSystem(PlanetController pc, DirectoryInfo dir, Vector3 origin) {
			this.pc = pc;
			this.origin = origin;
			loadFolder(dir);
		}
		
		public void setOrigin(Vector3 origin) {
			sun.transform.position = sun.transform.position - this.origin + origin;
			
			foreach (GameObject startCluster in startClusters) {
				startCluster.transform.position = startCluster.transform.position - this.origin + origin;
			}
			foreach (GameObject planet in planets) {
				planet.transform.position = planet.transform.position - this.origin + origin;
			}
			
			this.origin = origin;
		}
		
		private void loadFolder(DirectoryInfo dir) {
			this.dir = dir;
			dirs =  dir.GetDirectories();
			files = dir.GetFiles();
			
			startClusters = new GameObject[dirs.Length];
			planets = new GameObject[files.Length];
			
			sun =  createObject(dir.Name, pc.sunPrefab, null, origin);
			for (int i=0; i<startClusters.Length; i++) {
				startClusters[i] = createObject(dirs[i].Name, pc.starClusterPrefab, null, origin + new Vector3(3f,0f,(i+1)*(-30f)));
			}
			for (int i=0; i<planets.Length; i++) {
				string[] extention = files[i].Name.Split(new Char[] {'.'});
				Material planetMat = getMaterialFromType(extention[extention.Length-1]);
				planets[i] = createObject(files[i].Name, pc.planetPrefab, planetMat, origin + new Vector3(0f,0f,(i+1)*(-20f)));

				// set a random axis tilt
				float randomAngleX = UnityEngine.Random.Range(-0.5f,0.5f);
				float randomAngleZ = UnityEngine.Random.Range(-0.5f,0.5f);
				Vector3 upVector = new Vector3(Mathf.Cos(randomAngleX),1.0f,Mathf.Cos (randomAngleZ));
				planets[i].transform.FindChild("Planet").transform.rotation = Quaternion.LookRotation(Vector3.Cross(pc.transform.up,upVector),upVector);

				// set a random angular speed
				float randomAngular = UnityEngine.Random.Range(-0.5f,0.5f);
				planets[i].transform.FindChild("Planet").GetComponent<Rigidbody>().angularVelocity = randomAngular * upVector;
			}
			
			createObject (
				"Asteroids",
				pc.asteroidBeltPrefab,
				null,
				new Vector3(0f, 2f, (Mathf.Max (dirs.Length, files.Length)+1)*(-20f)));
		}
		
		public void removeAll() {
			Destroy (sun);
			foreach (GameObject startCluster in startClusters) {
				Destroy (startCluster);
			}
			foreach (GameObject planet in planets) {
				Destroy (planet);
			}
		}
			
		public void DrawNames (float playerPosition) {
			// find planets / star cluster whose name must be drawn
			
		}
		
		private GameObject createObject(String name, GameObject prefab, Material mat, Vector3 pos) {
			GameObject currentFile = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
			
			currentFile.name = name;
			if (mat != null) {
				Renderer rend = currentFile.GetComponent<Renderer> ();
				
				if (rend == null)
					rend = currentFile.transform.GetComponentInChildren<Renderer> ();
				
				rend.material = mat;
			}
			
			Transform nameObject = currentFile.transform.Find("Name");
			if (null != nameObject) {
				nameObject.GetComponent<TextMesh> ().text = name;
			} else {
				Debug.Log ("No Text child for object " + name);
			}
			
			return currentFile;
		}
		
		private Material getMaterialFromType(String ext) {
			switch (ext) {
			case "exe":
				return pc.executable;
			case "txt":
			case "cs":
				return pc.text;
			case "jpg":
			case "png":
			case "tga":
				return pc.image;
			case "avi":
				return pc.video;
			case "mp3":
				return pc.music;
			case "odf":
			case "doc":
			case "docx":
				return pc.document;
			case "obj":
				return pc.model3D;
			default:
				return pc.other;
			}
		}
	}
}