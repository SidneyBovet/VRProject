using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class planetController : MonoBehaviour {
	
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
	
	private DirectoryInfo dir;
	private DirectoryInfo[] dirs;
	private FileInfo[] files;
	
	private GameObject sun;
	private GameObject[] startClusters;
	private GameObject[] planets;
	
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
			planets[i] = createObject(files[i].Name, planetPrefab, executable, new Vector3(0f,0f,(i+1)*(-20f)));
		}

		createObject (
			"Asteroids",
			asteroidBeltPrefab,
			null,
			new Vector3(0f, 0f, (Mathf.Max (dirs.Length, files.Length)+1)*(-20f)));
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
			rend.material = mat;
		}

		Transform nameObject = currentFile.transform.Find("Name");
		if (null != nameObject) {
			nameObject.GetComponent<TextMesh> ().text = name;
		} else {
			Debug.Log ("No Text child");
		}

		return currentFile;
	}

	public void DrawNames (float playerPosition) {
		// find planets / star cluster whose name must be drawn

	}
}