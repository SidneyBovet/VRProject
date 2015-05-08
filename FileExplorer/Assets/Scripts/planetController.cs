using UnityEngine;
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

	private DirectoryInfo dir;
	private DirectoryInfo[] dirs;
	private FileInfo[] files;
	
	private GameObject sun;
	private GameObject[] startClusters;
	private GameObject[] planets;

	private bool canSelect = true;
	
	Vector3 origin = Vector3.zero;

	void Start () {
		loadFolder(new DirectoryInfo ("."));
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
	
	public bool changeSolarSystem(GameObject newSystem) {
		DirectoryInfo newDir = null;
		if(newSystem == sun) {
			newDir = dir.Parent;
		} else {
			for (int i=0; i<startClusters.Length; i++) {
				if(newSystem == startClusters[i]) {
					newDir = dirs[i];
					break;
				}
			}
		}
		if(newDir == null) {
			return false;
		}
		
		removeAll ();
		loadFolder (newDir);
		return true;
	}
	
	private void loadFolder(DirectoryInfo dir) {
		this.dir = dir;
		dirs =  dir.GetDirectories();
		files = dir.GetFiles();

		startClusters = new GameObject[dirs.Length];
		planets = new GameObject[files.Length];

		origin = Vector3.zero;
		sun =  createObject(dir.Name, sunPrefab, null, origin);
		for (int i=0; i<startClusters.Length; i++) {
			startClusters[i] = createObject(dirs[i].Name, starClusterPrefab, null, new Vector3(3f,0f,(i+1)*(-30f)));
		}
		for (int i=0; i<planets.Length; i++) {
			// determine the material to be used
			Material planetMat = null;
			string[] extention = files[i].Name.Split(new Char[] {'.'});
			switch (extention[extention.Length-1]) {
			case "exe":
				planetMat = executable;
				break;
			case "txt":
				planetMat = text;
				break;
			case "cs":
				planetMat = text;
				break;
			case "jpg":
				planetMat = image;
				break;
			case "png":
				planetMat = image;
				break;
			case "tga":
				planetMat = image;
				break;
			case "avi":
				planetMat = video;
				break;
			case "mp3":
				planetMat = music;
				break;
			case "odf":
				planetMat = document;
				break;
			case "doc":
				planetMat = document;
				break;
			case "docx":
				planetMat = document;
				break;
			case "obj":
				planetMat = model3D;
				break;
			default:
				planetMat = other;
				break;
			}

			planets[i] = createObject(files[i].Name, planetPrefab, planetMat, new Vector3(0f,0f,(i+1)*(-20f)));

			// set a random axis tilt
			float randomAngleX = UnityEngine.Random.Range(-0.5f,0.5f);
			float randomAngleZ = UnityEngine.Random.Range(-0.5f,0.5f);
			Vector3 upVector = new Vector3(Mathf.Cos(randomAngleX),1.0f,Mathf.Cos (randomAngleZ));
			planets[i].transform.FindChild("Planet").transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.up,upVector),upVector);

			// set a random angular speed
			float randomAngular = UnityEngine.Random.Range(-0.5f,0.5f);
			planets[i].transform.FindChild("Planet").GetComponent<Rigidbody>().angularVelocity = randomAngular * upVector;
		}

		createObject (
			"Asteroids",
			asteroidBeltPrefab,
			null,
			new Vector3(0f, 2f, (Mathf.Max (dirs.Length, files.Length)+1)*(-20f)));
	}
	
	private void removeAll() {
		Destroy (sun);
		foreach (GameObject startCluster in startClusters) {
			Destroy (startCluster);
		}
		foreach (GameObject planet in planets) {
			Destroy (planet);
		}
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

	public void DrawNames (float playerPosition) {
		// find planets / star cluster whose name must be drawn

	}

	public void Selection() {
		Collider selected = transform.Find ("/OVRPlayerController").GetComponent<UserController>().GetSelection();
		// TODO displaz file or cd to folder

		if (canSelect && selected != null && selected.CompareTag ("File")) {
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
		}
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
}