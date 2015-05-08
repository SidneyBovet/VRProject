using UnityEngine;
using System.Collections;

public class FileReader : MonoBehaviour {

	public static string ReadTextFile(string path) {
		string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");

		return text;
	}
}
