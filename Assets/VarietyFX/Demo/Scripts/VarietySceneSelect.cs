using UnityEngine;
using UnityEngine.SceneManagement;

namespace VarietyFX
{

public class VarietySceneSelect : MonoBehaviour
{
	public bool GUIHide = false;
	
    public void LoadSceneDemo1() { SceneManager.LoadScene("VarietyFX01"); }
	public void LoadSceneDemo2() { SceneManager.LoadScene("VarietyFX02"); }
	public void LoadSceneDemo3() { SceneManager.LoadScene("VarietyFX03"); }
	public void LoadSceneDemo4() { SceneManager.LoadScene("VarietyFX04"); }
	public void LoadSceneDemo5() { SceneManager.LoadScene("VarietyFX05"); }
	public void LoadSceneDemo6() { SceneManager.LoadScene("VarietyFX06"); }
	public void LoadSceneDemo7() { SceneManager.LoadScene("VarietyFX07"); }
	public void LoadSceneDemo8() { SceneManager.LoadScene("VarietyFX08"); }
	public void LoadSceneDemo9() { SceneManager.LoadScene("VarietyFX09"); }
	public void LoadSceneDemo10() { SceneManager.LoadScene("VarietyFX10"); }
	public void LoadSceneDemo11() { SceneManager.LoadScene("VarietyFX11"); }
	public void LoadSceneDemo12() { SceneManager.LoadScene("VarietyFX12"); }
	public void LoadSceneDemo13() { SceneManager.LoadScene("VarietyFX13"); }
	public void LoadSceneDemo14() { SceneManager.LoadScene("VarietyFX14"); }
	public void LoadSceneDemo15() { SceneManager.LoadScene("VarietyFX15"); }
	public void LoadSceneDemo16() { SceneManager.LoadScene("VarietyFX16"); }
	public void LoadSceneDemo17() { SceneManager.LoadScene("VarietyFX17"); }
	public void LoadSceneDemo18() { SceneManager.LoadScene("VarietyFX18"); }
	public void LoadSceneDemo19() { SceneManager.LoadScene("VarietyFX19"); }

void Update ()
	 {
 
     if(Input.GetKeyDown(KeyCode.J))
	 {
         GUIHide = !GUIHide;
     
         if (GUIHide)
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = true;
         }
     }
	 }
}
}