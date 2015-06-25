using UnityEngine;
using System.Collections;

public class SuitConfigurationsScript : MonoBehaviour{

	public static Vector3[] TWO = {new Vector3(0, 6.5f),
								   new Vector3(0, -6.5f)};

	public static Vector3[] THREE = {new Vector3(0, 0),
									 new Vector3(0, -8),
									 new Vector3(0, 8)};

	public static Vector3[] FOUR = {new Vector3(-4, 5.5f),
									new Vector3(4, 5.5f),
									new Vector3(4, -5.5f),
									new Vector3(-4, -5.5f)};

	public static Vector3[] FIVE = {new Vector3(0, 0),
									new Vector3(-4.5f, 6.5f),
									new Vector3(4.5f, 6.5f),
									new Vector3(4.5f, -6.5f),
									new Vector3(-4.5f, -6.5f)};

	public static Vector3[] SIX = {new Vector3(-4.25f, 6.75f),
								   new Vector3(4.25f, 6.75f),
								   new Vector3(4.25f, 0),
								   new Vector3(4.25f, -6.75f),
								   new Vector3(-4.25f, -6.75f),
								   new Vector3(-4.25f, 0)};

	public static Vector3[] SEVEN = {new Vector3(-4.25f, 6.75f),
									 new Vector3(4.25f, 6.75f),
									 new Vector3(4.25f, 0),
									 new Vector3(4.25f, -6.75f),
									 new Vector3(-4.25f, -6.75f),
									 new Vector3(-4.25f, 0),
									 new Vector3(0, 3.5f)};

	public static Vector3[] EIGHT = {new Vector3(-4.25f, 6.75f),
									 new Vector3(4.25f, 6.75f),
									 new Vector3(4.25f, 0),
									 new Vector3(4.25f, -6.75f),
									 new Vector3(-4.25f, -6.75f),
									 new Vector3(-4.25f, 0),
									 new Vector3(0, 3.5f),
									 new Vector3(0, -3.5f)};

	public static Vector3[] NINE = {new Vector3(-4.25f, 2.75f),
									new Vector3(-4.25f, 8.25f),
									new Vector3(4.25f, 8.25f),
									new Vector3(4.25f, 2.75f),
									new Vector3(4.25f, -2.75f),
									new Vector3(4.25f, -8.25f),
									new Vector3(-4.25f, -8.25f),
									new Vector3(-4.25f, -2.75f),
									new Vector3(0, 0)};

	public static Vector3[] TEN = {new Vector3(-4.25f, 2.75f),
								   new Vector3(-4.25f, 8.25f),
								   new Vector3(4.25f, 8.25f),
								   new Vector3(4.25f, 2.75f),
								   new Vector3(4.25f, -2.75f),
								   new Vector3(4.25f, -8.25f),
								   new Vector3(-4.25f, -8.25f),
								   new Vector3(-4.25f, -2.75f),
								   new Vector3(0, 5.5f),
								   new Vector3(0, -5.5f)};

	public static Vector3[][] ALL = {TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN};
}