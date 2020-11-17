using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordHaloController : MonoBehaviour
{
    // Radius of the halo
    public float CircleRadius = 100f;
    // Text to be rendered
    public string Text = "Hello World";
    // Input for the text
    public InputController InputController;
    // Extra space between each character
    public float CharacterSpacing = 20f;
    // Space between each new line
    public float LineSpacing = 22f;
    // Space taken for a " " 
    public float SpaceWidth = 8f;
    // Z coord of text when flattened
    public float FlatZ = 4f;

    // Speed of Exploding Anim
    public float ExplodeSpeed = 2f;

    // Animation curve for exploding anim
    public AnimationCurve ExplodeAnimationCurve;

    // Animation curve for retracting anim;
    public AnimationCurve RetractAnimationCurve;

    // Mesh renderer of face occlusion
    public MeshRenderer FaceMeshRenderer;

    // Material on the characters
    public Material CharacterMat;

    // Max eye brow color 
    public Color TargetColor;



    private bool isSpinning = true;
    private float charXLocation;
    private float charYLocation;
    private float maxXLocation;

    private Vector3 objScale;

    private List<WordHaloCharacter> placedCharacters = new List<WordHaloCharacter>();

    private void ListenToInputs()
    {
        InputController.TextInput.onValueChanged.AddListener(SetText);
        InputController.AddExplodeButtonDownEvent(delegate { AnimateEachExplode(); });
        InputController.AddExplodeButtonUpEvent(delegate { AnimateRetract(); });


    }

    private void UnsubscribeToInputs()
    {
        InputController.TextInput.onValueChanged.RemoveAllListeners();
    }

    private void Awake()
    {
        // Disable _Alphabets and all children under it to remove them from being seen.
        transform.Find("_Alphabets").gameObject.SetActive(false);

        if(InputController == null)
        {
            InputController = FindObjectOfType<InputController>();
        }

        InputController.Text = this.Text;
    }

    public void SetText(string newText)
    {
        if(newText != this.Text)
        {
            this.Text = InputController.Text;
            GenerateText();
        }
    }

    public void GenerateText()
    {
        // Clear the text first
        ResetText();

        // Check input field for non-empty string
        if (InputController.Text.Length > 0)
        {
            this.Text = InputController.Text;
        }

        // check all letters
        for (int ctr = 0; ctr <= Text.Length - 1; ctr++)
        {

            // dealing with linebreaks "\n"
            if (Text[ctr].ToString().ToCharArray()[0] == "\n"[0])
            {
                charXLocation = 0;
                charYLocation -= LineSpacing;
                continue;
            }

            string childObjectName = Text[ctr].ToString();


            if (childObjectName != " ")
            {

                GameObject LetterToShow;

                if (childObjectName == "/")
                {
                    LetterToShow = transform.Find("_Alphabets/" + "slash").gameObject; //special case for "/" since it cannot be used for obj name in fbx                 
                }
                else if (childObjectName == ".")
                {
                    LetterToShow = transform.Find("_Alphabets/" + "period").gameObject; //special case for "." - naming issue 
                }
                else
                {
                    LetterToShow = transform.Find("_Alphabets/" + childObjectName).gameObject;
                }

                AddLetter(LetterToShow);

                // find the width of the letter used
                Mesh mesh = LetterToShow.GetComponent<MeshFilter>().sharedMesh;
                Bounds bounds = mesh.bounds;
                charXLocation += (bounds.size.x);
                // handle edge case
                if(childObjectName == "I" || childObjectName == "i" || childObjectName == "l")
                {
                    charXLocation += 5f;
                }
            }
            else
            {
                charXLocation += SpaceWidth;
            }
        }

        // Calculates bounds 
        SetMaxCharPos();

    }

    private void AddLetter(GameObject LetterObject)
    {
        GameObject NewLetter = Instantiate(LetterObject, transform.position, transform.rotation) as GameObject;
        NewLetter.transform.parent = transform; // setting parent relationship

        // adding control component to the object. 
        WordHaloCharacter haloCharacter = NewLetter.AddComponent<WordHaloCharacter>();
        // storing it in local array.
        placedCharacters.Add(haloCharacter);


        // rename instantiated object
        NewLetter.name = LetterObject.name;

        // scale accoring to parent obj scale
        float newScaleX = NewLetter.transform.localScale.x * objScale.x;
        float newScaleY = NewLetter.transform.localScale.y * objScale.y;
        float newScaleZ = NewLetter.transform.localScale.z * objScale.z;

        NewLetter.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);

        // dealing with characters with a line down on the left (kerning, especially for use with multiple lines)
        if (charXLocation < 0.001f)
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
                charXLocation += (2f*objScale.magnitude);

        // position the new char
        // add a bit of angle padding
        float angle = Mathf.Deg2Rad * charXLocation * 1.25f;
        float xPosOnCircle = this.CircleRadius * Mathf.Cos(angle);
        float zPosOnCircle = this.CircleRadius * Mathf.Sin(angle);
        NewLetter.transform.localPosition = new Vector3(xPosOnCircle, charYLocation, zPosOnCircle);
        NewLetter.transform.localRotation = Quaternion.LookRotation(-1f * Vector3.Normalize(NewLetter.transform.localPosition));

        // store transform info in the char
        haloCharacter.localCirclePos = NewLetter.transform.localPosition;
        haloCharacter.localCircleRot = NewLetter.transform.localRotation;
        haloCharacter.localFlatPos = new Vector3(charXLocation*objScale.magnitude, charYLocation*objScale.magnitude, FlatZ);
        haloCharacter.localFlatRot = Quaternion.Euler(0f,180f,0f);

        // add a small space between words
        charXLocation += (CharacterSpacing);
    }

    void ResetText()
    {
        // refresh object scale
        objScale = transform.localScale;;


        // reset position
        charXLocation = 0f;
        charYLocation = 0f;

        // remove all previous created letters

        foreach (WordHaloCharacter placedChar in placedCharacters)
        {
            if (placedChar.name != "_Alphabets" && placedChar.name != transform.name && placedChar.transform.parent.name != "_Alphabets")
            {
                DestroyImmediate(placedChar.gameObject);
            }

        }

        placedCharacters.Clear();

    }

    // Start is called before the first frame update
    void Start()
    {
        ListenToInputs();
        GenerateText();
        isSpinning = true;

    }

    private void UpdateCharacterMaterial()
    {
        Color lerpedColor = Color.Lerp(Color.white, TargetColor, InputController.NormalizedBrowCoefficient);
        this.CharacterMat.SetColor("_Color",  lerpedColor);
    }

    // Update is called once per frame
    void Update()
    {
    
        if (this.isSpinning)
        {
            this.transform.Rotate(Vector3.up * Time.deltaTime * 100f);

        }

        this.UpdateCharacterMaterial();


        // Used for in-editor testing
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GenerateText();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AnimateEachExplode();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            AnimateRetract();
        }

    }

    private void SetMaxCharPos()
    {
        if (placedCharacters.Count == 0) return;
        float maxX = placedCharacters[0].localFlatPos.x;
        string charName = placedCharacters[0].name;

        foreach (WordHaloCharacter character in placedCharacters)
        {

            maxX = Mathf.Max(maxX, Mathf.Abs(character.localFlatPos.x));

        }

        maxXLocation = maxX;
    }

    private void AnimateRetract()
    {
        StartCoroutine(AnimateRetract(0.5f));
    }

    private void AnimateEachExplode()
    {
        float speed = ExplodeSpeed;
        Vector3 xOffset = Vector3.left * 0.5f * maxXLocation;
        isSpinning = false;

        //Hide the face mesh
        FaceMeshRenderer.enabled = false;

        foreach (WordHaloCharacter character in placedCharacters)
        {
            character.transform.parent = Camera.main.transform;
            // Store the starting position for lerping in camera space
            character.cameraCirclePos =
            Camera.main.transform.InverseTransformPoint(character.transform.position);
                
            // Need to shift so centered on screen
            Vector3 localGoalPos = character.localFlatPos + xOffset;
            character.AnimateExplode(localGoalPos, speed, ExplodeAnimationCurve);

        }
    }

    private IEnumerator AnimateRetract(float duration)
    {
        float elapsedTime = 0f;
        this.transform.localRotation = Quaternion.identity;

        foreach (WordHaloCharacter character in placedCharacters)
        {
            character.transform.parent = this.transform;
            // Store the starting position for lerping in local space
            character.localFlatPosTransformed =
            this.transform.InverseTransformPoint(character.transform.position);
        }
        while (elapsedTime < duration)
        {
            float t = RetractAnimationCurve.Evaluate(elapsedTime / duration);
            foreach (WordHaloCharacter character in placedCharacters)
            {
                character.transform.localPosition =
                    Vector3.Lerp(character.localFlatPosTransformed,
                                   character.localCirclePos, t);

                character.transform.localRotation =
                    Quaternion.Slerp(character.localFlatRot,
                                    character.localCircleRot, t);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.isSpinning = true;
        // Re-enable face mesh for occlusion
        this.FaceMeshRenderer.enabled = true;
    }

    private void OnDestroy()
    {
        UnsubscribeToInputs();
    }

}
