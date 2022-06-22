/* AbilityController
 * 
 * Sets the ability buttons to the abilities set by the Level.
 * 
 * Also controls which ability is active and ready to use.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public class AbilityController : MonoBehaviour
{
    //The Ability to be used.
    public Ability activeAbility;

    [Tooltip("The abilities to load into the level.")]
    public Ability[] abilities;// = new Ability[3];

    //The associated ability buttons.
    private Button[] buttons;

    //The icons for said buttons.
    private VisualElement[] icons;

    //The dark cooldown filter overlay
    private VisualElement[] cdFilter;

    //Cooldown times.
    private float[] cooldowns;
    private bool[] isReady;

    public Color originalColor;
    public StyleColor sColor;

    private void Start()
    {
        SetUI();
    }
    private void Update()
    {
        TriggerAbility();
        ButtonCooldownDisplay();
    }

    //Fades the button in after use.
    void ButtonCooldownDisplay()
    {
        //For each cooldown filter...
        for (int i = 0; i < cdFilter.Length; i++)
        {
            //Reset the beginning of the cooldown
            float startTime = cooldowns[i] - abilities[i].Cooldown;

            //What percent of the cooldown has elapsed?
            //Semi-Normalized. Counts down from 1-0(-infinity).
            var perc = -1 * ((Time.time - cooldowns[i]) / (cooldowns[i] - startTime));

            //Sets the opacity in accordance with the time remaining
            cdFilter[i].style.opacity = perc;
        }
    }

    //After cooldown time, flash the color to show it is ready.
    IEnumerator ButtonCooldown(int i)
    {
        yield return new WaitForSeconds(cooldowns[i]);
        buttons[i].style.backgroundColor = Color.white;
        yield return new WaitForSeconds(0.25f);
        buttons[i].style.backgroundColor = sColor;
    }

    //Instantiates the ability
    private void TriggerAbility()
    {
        if (activeAbility != null)
        {
            //Uses the abilities input Gesture
            activeAbility.Gesture();

            if (activeAbility.isReady == true)
            {
                //When ability is ready, trigger and remove it.
                //isReady MUST be set to false.
                activeAbility.isReady = false; //TODO: Change to prefab-driven instantiation.
                activeAbility.TriggerAbility();
                activeAbility = null;
            }
        }
    }

    //Sets the UI elements
    public void SetUI()
    {
        //Set up the Buttons
        var root = GetComponent<UIDocument>().rootVisualElement;
        buttons = new Button[abilities.Length];

        buttons[0] = root.Q<Button>("ability0");
        buttons[1] = root.Q<Button>("ability1");
        buttons[2] = root.Q<Button>("ability2");

        buttons[0].clicked += Pressed0;
        buttons[1].clicked += Pressed1;
        buttons[2].clicked += Pressed2;

        //Set up the Icons
        icons = new VisualElement[abilities.Length];
        icons[0] = root.Q<VisualElement>("abilityicon0");
        icons[1] = root.Q<VisualElement>("abilityicon1");
        icons[2] = root.Q<VisualElement>("abilityicon2");

        //Set up the Cooldown Filters
        cdFilter = new VisualElement[abilities.Length];
        cdFilter[0] = root.Q<VisualElement>("cdfilter0");
        cdFilter[1] = root.Q<VisualElement>("cdfilter1");
        cdFilter[2] = root.Q<VisualElement>("cdfilter2");

        //Set cooldowns and icons
        cooldowns = new float[abilities.Length];
        isReady = new bool[abilities.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            cooldowns[i] = abilities[i].Cooldown;
            isReady[i] = false;
            icons[i].style.backgroundImage = new StyleBackground(abilities[i].Icon);

            //Starts the button cooldown process at level load.
            StartCoroutine(ButtonCooldown(i));
        }
        sColor = buttons[0].style.backgroundColor;
    }

    //If the ability is off cooldown, Select it
    void Pressed0()
    {
        if (Time.time > cooldowns[0])
            StartCoroutine(AbilitySelected(0));
    }
    void Pressed1()
    {
        if (Time.time > cooldowns[1])
            StartCoroutine(AbilitySelected(1));
    }
    void Pressed2()
    {
        if (Time.time > cooldowns[2])
            StartCoroutine(AbilitySelected(2));
    }

    //Sets ActiveAbility to selected ability and reset its cooldown
    IEnumerator AbilitySelected(int a)
    {        
        cooldowns[a] = Time.time + abilities[a].Cooldown;
        yield return new WaitForEndOfFrame();
        activeAbility = abilities[a];
    }
}
