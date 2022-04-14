using System.Collections;
using System;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeantweenEditor : MonoBehaviour
{
    public bool Otomatis;
    public string animasiPlayed;
    //public List<bool> repeatAnim = new List<bool>();
    //public List<bool> repeatAnimDeleted = new List<bool>();
    public List<string> animNames = new List<string>();
    //public List<string> animNamesDeleted = new List<string>();
    public List<LeantweenMainDataContainer> AnimMainDatas = new List<LeantweenMainDataContainer>();
    //public List<LeantweenMainDataContainer> AnimMainDatasDeleted = new List<LeantweenMainDataContainer>();
    public List<float> timesMax = new List<float>();
   // private bool Ulangi;
    private int indexBefore;

	Quaternion quat;
	Vector3 localPos;
	Vector3 localScale;

	void Awake()
	{
		quat = transform.rotation;
		localPos = transform.localPosition;
		localScale = transform.localScale;
	}

    public void OnEnable()
    {
        if (Otomatis)
            Play(animasiPlayed);
    }

    private void OnDisable()
    {
		if (LeanTween.isTweening (gameObject)) {
			LeanTween.cancel (gameObject);
		}
        StopAllCoroutines();
		transform.rotation = quat;
		transform.localPosition = localPos;
		transform.localScale = localScale;
    }
		
    private IEnumerator RepeatAnim(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
       // if (Ulangi)
        //{
            StartCoroutine(PlayAnim(index, 0));
      //  }
    }

    public void AddAnimationState(int index)
    {
        LeantweenDatabaseContainer lEnum = new LeantweenDatabaseContainer();
		lEnum.theObject = gameObject;
        AnimMainDatas[index].data.Add(lEnum);
    }

    public void DeleteAnimationState(int index)
    {
        AnimMainDatas[index].data.RemoveAt(AnimMainDatas[index].data.Count - 1);
    }

    public void AddSprite(int index1, int index2, int index3)
    {
        SpriteDatabase s = new SpriteDatabase();
        AnimMainDatas[index1].data[index2].data[index3].sprites.Add(s);
    }

    public List<SpriteDatabase> Sprites(int index1, int index2, int index3)
    {
        return AnimMainDatas[index1].data[index2].data[index3].sprites;
    }

    public int SpriteCount(int index1, int index2, int index3)
    {
        return AnimMainDatas[index1].data[index2].data[index3].sprites.Count;
    }

    public void DeleteSprite(int index1, int index2, int index3)
    {
        List<SpriteDatabase> s = AnimMainDatas[index1].data[index2].data[index3].sprites;
        s.RemoveAt(s.Count - 1);
    }

    public void AddAnimation()
    {
        LeantweenMainDataContainer lEnum = new LeantweenMainDataContainer();
        AnimMainDatas.Add(lEnum);
        animNames.Add("");
        //repeatAnim.Add(false);
    }

    public void DeleteAnimation()
    {
        //AnimMainDatasDeleted.Add(AnimMainDatas[AnimMainDatas.Count - 1]);
        AnimMainDatas.RemoveAt(AnimMainDatas.Count - 1);
       // animNamesDeleted.Add(animNames[animNames.Count - 1]);
        animNames.RemoveAt(animNames.Count - 1);
       // repeatAnimDeleted.Add(repeatAnim[repeatAnim.Count - 1]);
        //repeatAnim.RemoveAt(repeatAnim.Count - 1);
    }

    public void DuplicateGroupAnimation()
    {
        LeantweenMainDataContainer lEnum = AnimMainDatas[AnimMainDatas.Count - 1];
        AnimMainDatas.Add(lEnum);
        string s = animNames[animNames.Count - 1];
        animNames.Add(s);
        //bool b = repeatAnim[repeatAnim.Count - 1];
        //repeatAnim.Add(b);
    }

    /*public void UndoAnimation()
    {
        AnimMainDatas.Add(AnimMainDatasDeleted[AnimMainDatasDeleted.Count - 1]);
        AnimMainDatasDeleted.RemoveAt(AnimMainDatasDeleted.Count - 1);
        animNames.Add(animNamesDeleted[animNamesDeleted.Count - 1]);
        animNamesDeleted.RemoveAt(animNamesDeleted.Count - 1);
        repeatAnim.Add(repeatAnimDeleted[repeatAnimDeleted.Count - 1]);
        repeatAnimDeleted.RemoveAt(repeatAnimDeleted.Count - 1);
    }*/

    public void ClearAllAnimationAndCache()
    {
       // AnimMainDatasDeleted.Clear();
        AnimMainDatas.Clear();
       // animNamesDeleted.Clear();
        animNames.Clear();
       // repeatAnimDeleted.Clear();
        //repeatAnim.Clear();
    }

    public void Play(string animationName)
    {
        if (animNames.Contains(animationName))
            StartCoroutine(PlayAnim(animNames.IndexOf(animationName), 0));
        else
            print("NAMA ANIMASI TIDAK ADA");
    }

    public void Play(string animationName, float delay)
    {
        if (animNames.Contains(animationName))
            StartCoroutine(PlayAnim(animNames.IndexOf(animationName), delay));
        else
            print("NAMA ANIMASI TIDAK ADA");
    }

    public void Play(int index)
    {
        if (index < AnimMainDatas.Count)
            StartCoroutine(PlayAnim(index, 0));
        else
            print("INDEX LEBIH");
    }

    public void Play(int index, float delay)
    {
        if (index < AnimMainDatas.Count)
            StartCoroutine(PlayAnim(index, delay));
        else
            print("INDEX LEBIH");
    }

    public void PlayAllAnim()
    {
        int count = AnimMainDatas.Count;
        for (int i = 0; i < count; i++)
        {
            StartCoroutine(SetAnim(AnimMainDatas[i]));
        }
    }

    private IEnumerator PlayAnim(int index, float delay)
    {
        if (index != indexBefore)
            StopCoroutine("RepeatAnim");
        indexBefore = index;
        yield return new WaitForSeconds(delay);
        StartCoroutine(SetAnim(AnimMainDatas[index]));
        //if (repeatAnim[index])
       // {
            float timeRepeat = 1;
            if (timesMax.Count > index)
            {
                timeRepeat = timesMax[index];
            }
            else
            {
                List<float> totalWaktu = new List<float>();
                int totalAnim = AnimMainDatas.Count;
                for (int i = 0; i < totalAnim; i++)
                {
                    int totalAnimdatabaseContainer = AnimMainDatas[i].data.Count;
                    for (int k = 0; k < totalAnimdatabaseContainer; k++)
                    {
                        int totalAnimdatabase = AnimMainDatas[i].data[k].data.Count;
                        for (int j = 0; j < totalAnimdatabase; j++)
                        {
                            totalWaktu.Add(AnimMainDatas[i].data[k].data[j].time + AnimMainDatas[i].data[k].data[j].delay);
                        }
                    }
                }
                timesMax.Add(totalWaktu.Max());
                timeRepeat = timesMax[timesMax.Count - 1];
            }
           // Ulangi = true;
            //StartCoroutine(RepeatAnim(index, timeRepeat));
        //}
        //else
        //    Ulangi = false;
    }

    private IEnumerator SetAnim(LeantweenMainDataContainer mainData)
    {
        yield return null;
        int mainCount = mainData.data.Count;
        for (int k = 0; k < mainCount; k++)
        {
            LeantweenDatabaseContainer data = mainData.data[k];
            int dataCount = data.data.Count;
            for (int i = 0; i < dataCount; i++)
            {
                GameObject target = data.theObject;
                if (!target)
                {
                    print("Objek kosong");
                    yield return null;
                }
                LeantweenDatabase d = data.data[i];
                switch ((int)d.typeOfAnimation)
                {
                    case 1:
						StartCoroutine(Move(target, d, d.delay, data.repeatType));
                        break;

                    case 2:
						StartCoroutine(Rotation(target, d, d.delay, data.repeatType));
                        break;

                    case 3:
						StartCoroutine(Scale(target, d, d.delay, data.repeatType));
                        break;

                    case 4:
						StartCoroutine(Color(target, d, d.delay, data.repeatType));
                        break;

                    case 5:
						StartCoroutine(ColorText(target, d, d.delay, data.repeatType));
                        break;

                    case 6:
                        StartCoroutine(DisableObject(target, d.delay));
                        break;

                    case 7:
                        CanvasAlpha(target, d);
                        break;

                    case 8:
                        StartCoroutine(SpriteAnim(target, d.sprites, d.delay));
                        break;
                }
            }
        }
    }

	private IEnumerator Move(GameObject target, LeantweenDatabase database, float delay, RepeatType rt)
	{
		yield return new WaitForSeconds(delay);
		switch (rt) {
		case RepeatType.NONE:
			LeanTween.moveLocal(target, database.toPosition, database.time).setEase(database.typeEase).setFrom(database.fromPosition);
			break;
		case RepeatType.CLAMP:
			LeanTween.moveLocal(target, database.toPosition, database.time).setEase(database.typeEase).setFrom(database.fromPosition).setLoopClamp();
			break;
		case RepeatType.PingPong:
			LeanTween.moveLocal(target, database.toPosition, database.time).setEase(database.typeEase).setFrom(database.fromPosition).setLoopPingPong();
			break;
		}
    }

	private IEnumerator Rotation(GameObject target, LeantweenDatabase d, float delay, RepeatType rt)
	{
		yield return new WaitForSeconds(delay);
		switch (rt) {
		case RepeatType.NONE:
			LeanTween.rotateLocal(target, d.toRotation, d.time).setEase(d.typeEase).setFrom(d.fromRotation);
			break;
		case RepeatType.CLAMP:
			LeanTween.rotateLocal(target, d.toRotation, d.time).setEase(d.typeEase).setFrom(d.fromRotation).setLoopClamp();
			break;
		case RepeatType.PingPong:
			LeanTween.rotateLocal(target, d.toRotation, d.time).setEase(d.typeEase).setFrom(d.fromRotation).setLoopPingPong();
			break;
		}
    }

	private IEnumerator Scale(GameObject target, LeantweenDatabase d, float delay, RepeatType rt)
	{
		yield return new WaitForSeconds(delay);
		switch (rt) {
		case RepeatType.NONE:
			LeanTween.scale(target, d.toScale, d.time).setEase(d.typeEase).setFrom(d.fromScale);
			break;
		case RepeatType.CLAMP:
			LeanTween.scale(target, d.toScale, d.time).setEase(d.typeEase).setFrom(d.fromScale).setLoopClamp();
			break;
		case RepeatType.PingPong:
			LeanTween.scale(target, d.toScale, d.time).setEase(d.typeEase).setFrom(d.fromScale).setLoopPingPong();
			break;
		}
    }

	private IEnumerator Color(GameObject target, LeantweenDatabase d, float delay, RepeatType rt)
	{
		yield return new WaitForSeconds(delay);
		switch (rt) {
		case RepeatType.NONE:
			LeanTween.color(target, d.toColor, d.time).setEase(d.typeEase).setFromColor(d.fromColor);
			break;
		case RepeatType.CLAMP:
			LeanTween.color(target, d.toColor, d.time).setEase(d.typeEase).setFromColor(d.fromColor).setLoopClamp();
			break;
		case RepeatType.PingPong:
			LeanTween.color(target, d.toColor, d.time).setEase(d.typeEase).setFromColor(d.fromColor).setLoopPingPong();
			break;
		}
        /*if (target.GetComponent<Image>() != null)
            target.GetComponent<Image>().color = d.fromColor;
        else
            target.GetComponent<SpriteRenderer>().color = d.fromColor;*/
    }

	private IEnumerator ColorText(GameObject target, LeantweenDatabase d, float delay, RepeatType rt)
    {
        yield return new WaitForSeconds(delay);
		switch (rt) {
		case RepeatType.NONE:
			LeanTween.colorText (target.GetComponent<RectTransform> (), d.toColorText, d.time).setEase (d.typeEase).setFromColor (d.fromColorText);
			break;
		case RepeatType.CLAMP:
			LeanTween.colorText (target.GetComponent<RectTransform> (), d.toColorText, d.time).setEase (d.typeEase).setFromColor (d.fromColorText).setLoopClamp();
			break;
		case RepeatType.PingPong:
			LeanTween.colorText (target.GetComponent<RectTransform> (), d.toColorText, d.time).setEase (d.typeEase).setFromColor (d.fromColorText).setLoopPingPong();
			break;
		}
    }

    private void CanvasAlpha(GameObject objek, LeantweenDatabase d)
    {
        CanvasGroup cg = objek.GetComponent<CanvasGroup>();
        if(cg!=null)
        {
            LeanTween.alphaCanvas(cg, d.toA, d.time).setFrom(d.fromA).setDelay(d.delay);
        }
    }

    private IEnumerator DisableObject(GameObject objek, float delay)
    {
        yield return new WaitForSeconds(delay);
        objek.SetActive(false);
    }

    private IEnumerator SpriteAnim(GameObject objek, List<SpriteDatabase> database, float delay)
    {
        yield return new WaitForSeconds(delay);
        Sprite s = null;
        if (objek.GetComponent<SpriteRenderer>() != null)
            s = objek.GetComponent<SpriteRenderer>().sprite;
        else
            s = objek.GetComponent<Image>().sprite;
        int count = database.Count;
        for (int i = 0; i < count; i++)
        {
            s = database[i].sprite;
            yield return new WaitForSeconds(database[i].time);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(LeantweenEditor))]
public class LeanEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LeantweenEditor theTarget = (LeantweenEditor)target;
        int count = theTarget.AnimMainDatas.Count;
        if (count > 0)
        {
            theTarget.Otomatis = EditorGUILayout.Toggle("Play OnEnable", theTarget.Otomatis);
            if (theTarget.Otomatis)
            {
                EditorGUILayout.BeginHorizontal();
                theTarget.animasiPlayed = EditorGUILayout.TextField("Played an animation", theTarget.animasiPlayed);
                if (GUILayout.Button("Play", GUILayout.Height(25f), GUILayout.Width(35f)))
                {
                    theTarget.OnEnable();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            for (int k = 0; k < count; k++)
            {
                LeantweenMainDataContainer mainData = theTarget.AnimMainDatas[k];
                EditorGUILayout.BeginHorizontal();
                theTarget.animNames[k] = EditorGUILayout.TextField("Group Name", theTarget.animNames[k]);
                //if (theTarget.animNames[k].Length > 0)
               // {
               //     theTarget.repeatAnim[k] = EditorGUILayout.Toggle("Repeat", theTarget.repeatAnim[k]);
               // }
                EditorGUILayout.EndHorizontal();
                if (theTarget.animNames[k].Length > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Object", GUILayout.Width(80f), GUILayout.Height(30f)))
                    {
                        theTarget.AddAnimationState(k);
                    }
                    int countDatabase = mainData.data.Count;
                    if (countDatabase > 0)
                    {
                        GUI.color = Color.red;
                        if (GUILayout.Button("Delete this Object", GUILayout.Width(80f), GUILayout.Height(30f)))
                        {
                            theTarget.DeleteAnimationState(k);
                        }
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                int dataCount = mainData.data.Count;
                for (int i = 0; i < dataCount; i++)
                {
                    LeantweenDatabaseContainer ed = mainData.data[i];
                    bool allowSceneObjects = !EditorUtility.IsPersistent(target);
                    ed.theObject = (GameObject)EditorGUILayout.ObjectField("Object", ed.theObject,
                        typeof(GameObject), allowSceneObjects);
                    int length = ed.data.Count;
                    for (int j = 0; j < length; j++)
                    {
                        LeantweenDatabase data = ed.data[j];
                        EditorGUILayout.BeginHorizontal();
                        data.typeOfAnimation = (TargetAnimation)EditorGUILayout.EnumPopup(data.typeOfAnimation, GUILayout.Width(80f));
						if (data.typeOfAnimation != TargetAnimation.NONE) {
							data.typeEase = (LeanTweenType)EditorGUILayout.EnumPopup (data.typeEase, GUILayout.Width (80f));
							if (data.typeOfAnimation != TargetAnimation.DISABLEOBJECT || data.typeOfAnimation != TargetAnimation.SPRITE)
							{
								ed.repeatType = (RepeatType)EditorGUILayout.EnumPopup(ed.repeatType, GUILayout.Width(80f));
							}
                        }
                        EditorGUILayout.EndHorizontal();
                        if (data.typeOfAnimation != TargetAnimation.NONE)
                        {
                            switch ((int)data.typeOfAnimation)
                            {
                                case 1:
                                    EditorGUILayout.BeginHorizontal();
                                    data.time = EditorGUILayout.FloatField("Total Time", data.time);
                                    data.delay = EditorGUILayout.FloatField("Delay Time", data.delay);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    data.fromPosition = EditorGUILayout.Vector3Field("From Position", data.fromPosition, GUILayout.Width(350f));
                                    if (GUILayout.Button("Set Auto", GUILayout.Width(70f)))
                                    {
                                        data.fromPosition = ed.theObject.transform.localPosition;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    data.toPosition = EditorGUILayout.Vector3Field("To Position", data.toPosition, GUILayout.Width(350f));
                                    if (GUILayout.Button("Set Auto", GUILayout.Width(70f)))
                                    {
                                        data.toPosition = ed.theObject.transform.localPosition;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;

                                case 2:
                                    EditorGUILayout.BeginHorizontal();
                                    data.time = EditorGUILayout.FloatField("Total Time", data.time);
                                    data.delay = EditorGUILayout.FloatField("Delay Time", data.delay);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    data.fromRotation = EditorGUILayout.Vector3Field("From Rotation", data.fromRotation, GUILayout.Width(350f));
                                    if (GUILayout.Button("Set Auto", GUILayout.Width(70f)))
                                    {
                                        data.fromRotation = ed.theObject.transform.localEulerAngles;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    data.toRotation = EditorGUILayout.Vector3Field("To Rotation", data.toRotation, GUILayout.Width(350f));
                                    if (GUILayout.Button("Set Auto", GUILayout.Width(70f)))
                                    {
                                        data.toRotation = ed.theObject.transform.localEulerAngles;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;

                                case 3:
                                    EditorGUILayout.BeginHorizontal();
                                    data.time = EditorGUILayout.FloatField("Total Time", data.time);
                                    data.delay = EditorGUILayout.FloatField("Delay Time", data.delay);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    data.fromScale = EditorGUILayout.Vector3Field("From Scale", data.fromScale, GUILayout.Width(350f));
                                    if (GUILayout.Button("Set Auto", GUILayout.Width(70f)))
                                    {
                                        data.fromScale = ed.theObject.transform.localScale;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    data.toScale = EditorGUILayout.Vector3Field("To Scale", data.toScale, GUILayout.Width(350f));
                                    if (GUILayout.Button("Set Auto", GUILayout.Width(70f)))
                                    {
                                        data.toScale = ed.theObject.transform.localScale;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;

                                case 4:
                                    EditorGUILayout.BeginHorizontal();
									data.time = EditorGUILayout.FloatField("Total Time", data.time);
									data.delay = EditorGUILayout.FloatField("Delay Time", data.delay);
                                    EditorGUILayout.EndHorizontal();
									data.fromColor = EditorGUILayout.ColorField("From Color", data.fromColor);
									data.toColor = EditorGUILayout.ColorField("To Color", data.toColor);
                                    break;

                                case 5:
                                    EditorGUILayout.BeginHorizontal();
                                    data.time = EditorGUILayout.FloatField("Total Time", data.time);
                                    data.delay = EditorGUILayout.FloatField("Delay Time", data.delay);
                                    EditorGUILayout.EndHorizontal();
                                    data.fromColorText = EditorGUILayout.ColorField("From Color", data.fromColorText);
                                    data.toColorText = EditorGUILayout.ColorField("To Color", data.toColorText);
                                    break;

                                case 6:
                                    data.delay = EditorGUILayout.FloatField("Time to Deactive", data.delay);
                                    break;

                                case 7:
                                    EditorGUILayout.BeginHorizontal();
                                    data.time = EditorGUILayout.FloatField("Total Time", data.time);
                                    data.delay = EditorGUILayout.FloatField("Delay Time", data.delay);
                                    EditorGUILayout.EndHorizontal();
                                    data.fromA = EditorGUILayout.FloatField("From Alpha", data.fromA);
                                    data.toA = EditorGUILayout.FloatField("To Alpha", data.toA);
                                    break;
                                case 8:
                                    int spriteCount = theTarget.SpriteCount(k, i, j);
                                    for (int m = 0; m < spriteCount; m++)
                                    {
                                        theTarget.Sprites(k, i, j)[m].sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", theTarget.Sprites(k, i, j)[m].sprite, typeof(Sprite), allowSceneObjects);
                                        theTarget.Sprites(k, i, j)[m].time = EditorGUILayout.FloatField("Waktu berganti Sprite", theTarget.Sprites(k, i, j)[m].time);
                                    }
                                    if (GUILayout.Button("+", GUILayout.Height(40f), GUILayout.Width(40f)))
                                    {
                                        theTarget.AddSprite(k, i, j);
                                    }
                                    if (theTarget.SpriteCount(k, i, j) > 0)
                                    {
                                        if (GUILayout.Button("-", GUILayout.Height(40f), GUILayout.Width(40f)))
                                        {
                                            theTarget.DeleteSprite(k, i, j);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("Add Anim", GUILayout.Width(80f), GUILayout.Height(35f)))
                    {
                        LeantweenDatabase d = new LeantweenDatabase();
                        ed.data.Add(d);
                    }
                    GUI.color = Color.white;
                    if (length > 0)
                    {
                        GUI.color = Color.magenta;
                        if (GUILayout.Button("Delete Anim", GUILayout.Width(80f), GUILayout.Height(35f)))
                        {
                            ed.data.RemoveAt(length - 1);
                        }
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add", GUILayout.Width(70f), GUILayout.Height(40f)))
        {
            theTarget.AddAnimation();
        }
        if (count > 0)
        {
            if (GUILayout.Button("Duplicate", GUILayout.Width(70f), GUILayout.Height(40f)))
            {
                theTarget.DuplicateGroupAnimation();
            }
            GUI.color = Color.red;
            if (GUILayout.Button("Delete", GUILayout.Width(70f), GUILayout.Height(40f)))
            {
                theTarget.DeleteAnimation();
            }
            GUI.color = Color.white;
        }
        /*int count2 = theTarget.AnimMainDatasDeleted.Count;
        if (count2 > 0)
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Undo", GUILayout.Width(70f), GUILayout.Height(40f)))
            {
                theTarget.UndoAnimation();
            }
            GUI.color = Color.red;
            if (GUILayout.Button("Delete All", GUILayout.Width(70f), GUILayout.Height(40f)))
            {
                theTarget.ClearAllAnimationAndCache();
            }
            GUI.color = Color.white;
        }*/
        EditorGUILayout.EndHorizontal();
    }
}

#endif

[Serializable]
public class LeantweenMainDataContainer
{
    public string animationName;
    public List<LeantweenDatabaseContainer> data = new List<LeantweenDatabaseContainer>();
}

[Serializable]
public class LeantweenDatabaseContainer
{
    public GameObject theObject;
	public RepeatType repeatType = RepeatType.NONE;
    public List<LeantweenDatabase> data = new List<LeantweenDatabase>();
}

[Serializable]
public class LeantweenDatabase
{
    public TargetAnimation typeOfAnimation;
    public LeanTweenType typeEase;
    public Vector3 fromPosition, toPosition, fromRotation, toRotation, fromScale, toScale;
    public Color fromColor, toColor, fromColorText, toColorText;
    public List<SpriteDatabase> sprites = new List<SpriteDatabase>();
    public float time, delay, fromA, toA;
}

[Serializable]
public class SpriteDatabase
{
    public Sprite sprite;
    public float time;
}

public enum TargetAnimation
{
    NONE = 0,
    MOVE = 1,
    ROTATION = 2,
    SCALE = 3,
    COLOR = 4,
    COLORTEXT = 5,
    DISABLEOBJECT = 6,
    CANVAS_ALPHA = 7,
    SPRITE = 8
}


public enum RepeatType
{
	NONE,
	CLAMP,
	PingPong
}