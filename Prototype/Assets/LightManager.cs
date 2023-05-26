using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LightType = UnityEngine.LightType;

public class LightManager : MonoBehaviour
{
    [Serializable]
    private struct LightStruct
    {
        public GameObject Obj;
        public float Range;
        public float Angle;
        public float Intensity;
    }
    [Serializable]
    private struct CharacterStruct
    {
        public GameObject Obj;
        public float LightValue;
        public MonoBehaviour MB;
    }

    [SerializeField] private Light[] _lightGameObjects; 
    [SerializeField] private List<LightStruct> _lights;
    [SerializeField] private List<CharacterStruct> _characters;
    [SerializeField] private bool _updatingList;

    private void Awake()
    {
        _lights = new List<LightStruct>();
        _lightGameObjects = FindObjectsOfType<Light>();
        foreach (var light in _lightGameObjects)
        {
            if (light.type == LightType.Directional) continue;
            var newStruct = new LightStruct();
            _lights.Add(InitLightStruct(light, newStruct));
        }

        var lightables = FindObjectsOfType<MonoBehaviour>().OfType<ILightable>();


        //Adds all characters in game to a list of CharacterStructs
        foreach (var obj in lightables)
        {
            var lightable = obj as MonoBehaviour;
            if (lightable == null) continue;
            var newChar = new CharacterStruct();
            _characters.Add(InitCharacterStruct(lightable, newChar));
        }
    }

    private static LightStruct InitLightStruct(Light light, LightStruct newStruct) //Sets the data for the light struct to be added to a list
    {
        newStruct.Obj = light.gameObject;
        newStruct.Range = light.range;
        newStruct.Angle = light.type switch
        {
            LightType.Spot => light.spotAngle / 2,
            LightType.Point => 360f / 2,
            _ => 180f / 2
        };
        newStruct.Intensity = light.intensity;
        return newStruct;
    }

    private static CharacterStruct InitCharacterStruct(MonoBehaviour character, CharacterStruct newChar) //Sets the data for the character struct.
    {
        newChar.Obj = character.gameObject;
        newChar.LightValue = 0f;
        newChar.MB = character;
        return newChar;
    }

    private void Start()
    {
        StartCoroutine(CheckRoutine()); //initializes the coroutine to begin managing lights.
    }

    /*
    private void FixedUpdate()
    {
        NullCheck();
        if (!_updatingList)
        {
            CheckDistance();
        }
    }
    */

    private IEnumerator CheckRoutine() //Runs a check 5 times per second. Will not check while the lists are being updated.
    {
        var delay = 0.2f;
        var wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            NullCheck();
            if (!_updatingList)
            {
                CheckDistance();
            }
        }
    }

    private void NullCheck() //Remove any null Characters from the list
    {
        for (var i = 0; i < _characters.Count; i++)
        {
            if (_characters[i].Obj == null)
            {
                RemoveChar(i);
            }
        }
    }

    private void RemoveChar(int index) //Removes a character at the index.
    {
        _updatingList = true;
        _characters.RemoveAt(index);
        _updatingList = false;
    }

    private void CheckDistance() //Check the distance of characters from the lights and update the light values when close enough.
    {
        for (var i = 0; i < _characters.Count; i++) //loop through every character
        {
            if (_characters[i].Obj == null) return; //ignore characters that are null. 
            var lightValue = _characters[i].LightValue;
            var closestLightDistance = 0f;
            var charPosition = _characters[i].Obj.tag == "Player" ? _characters[i].Obj.transform.GetChild(0).position : _characters[i].Obj.transform.position;
            var closestLightStruct = new LightStruct();
            for (var j = 0; j < _lights.Count; j++) //loop through every light
            {
                if (!_lights[j].Obj.activeSelf) continue; //Active check, currently unimplemented in prototype but would allow for inclusion of some realtime GI.

                var range = _lights[j].Range; 
                var lightPosition = _lights[j].Obj.transform.position;
                var distance = ReturnDistance(lightPosition, charPosition); //get distance to the light
                
                if (distance > range) continue; //if out of range, then continue

                if (!IsInAngle(_characters[i], _lights[j])) continue;

                var isClosestLight = closestLightDistance == 0 || distance < closestLightDistance;

                if (!isClosestLight) continue; //move on to the next light if the current is not closest

                Debug.DrawLine(charPosition, lightPosition, Color.blue);
                closestLightDistance = distance;
                closestLightStruct = _lights[j];
            }

            if (closestLightStruct.Obj == null) //if there is no closest light, set the character's light value to 0. They are in darkness.
            {
                _characters[i] = UpdateCharacterLightValue(_characters[i], 0);
                continue;
            }
            
            _characters[i] = FireRayCast(lightValue, _characters[i], closestLightStruct, closestLightDistance,
                closestLightStruct.Obj.transform.position, charPosition); //Fire a ray to the character to determine light value.
        }
    }

    private static CharacterStruct FireRayCast(float lightValue, CharacterStruct character, LightStruct light, float distance, Vector3 origin, Vector3 target) //returns an updated character struct. Determines how lit the character is based on distance to the light.
    {
        var charMaskLayer = LayerMask.LayerToName(character.Obj.layer);
        var Mask = LayerMask.GetMask(charMaskLayer) | LayerMask.GetMask("Environment");
        var direction = target - origin;
        if (Physics.Raycast(origin, direction, out var hit, light.Range, Mask))
        {
            var objTag = hit.transform.tag;
            Debug.DrawLine(origin, hit.point, Color.red);
            if (objTag == character.Obj.tag)
            {
                lightValue = CalculateLightValue(distance, light.Range, light.Intensity);
                Debug.DrawLine(origin, hit.point, Color.green);
            }
            else
                lightValue = 0;
        }
        return UpdateCharacterLightValue(character, lightValue);
    }


    private static bool IsInAngle(CharacterStruct character, LightStruct light) //Returns a true/false value if the character is not in the correct angle of the light source.
    {
        var cosAngle = Vector3.Dot(
            (character.Obj.transform.position - light.Obj.transform.position).normalized, light.Obj.transform.forward);
        var angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        return angle < light.Angle;
    }

    private static bool IsInAngle(Vector3 targetPosition, Transform lightTransform, float lightAngle) //Returns a true/false value if the character is not in the correct angle of the light source.
    {
        var cosAngle = Vector3.Dot((targetPosition - lightTransform.position).normalized, lightTransform.forward);
        var angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        return angle < lightAngle;
    }

    private static float CalculateLightValue(float distance, float range, float intensity) //Calculates the light value for the character to be updated. Scales the value to a 0-100 value. 0 being far away, 100 being up close. 
    {
        var lightValue = distance / range;
        var difference =(Mathf.Round((1 - lightValue) * 100f) * intensity);
        lightValue = Mathf.Clamp(difference, 0, 100f);
        return lightValue;
    }

    private static float ReturnDistance(Vector3 lightPos, Vector3 targetPos) //returns the distance between 2 points. 
    {
        var distance = Vector3.Distance(lightPos,targetPos);
        return distance;
    }

    private static CharacterStruct UpdateCharacterLightValue(CharacterStruct character, float lightValue) //replaces a specified struct. with new values. 
    {
        var newChar = character;
        newChar.LightValue = lightValue;
        if(newChar.MB != null)
            newChar.MB.GetComponent<ILightable>().HandleHitByLight((int)lightValue);
        return newChar;
    }

    public bool CheckValidSpot(GameObject obj, bool valid) //used to check if the target location is in light or dark. Follows same structure as checking characters against lights. Impacts Telerport and Shadow Pull abilities.
    {
        var objPos = obj.transform.position;
        var closestLightStruct = new LightStruct();
        var closestLightDistance = 0f;
        for (var i = 0; i < _lights.Count; i++)
        {
            if (!_lights[i].Obj.activeSelf) continue;
            var lightPosition = _lights[i].Obj.transform.position;
            var range = _lights[i].Range;
            var distance = ReturnDistance(lightPosition, objPos);
            if (distance > range) continue;

            if (!IsInAngle(objPos, _lights[i].Obj.transform, _lights[i].Angle)) continue;

            var isClosestLight = closestLightDistance == 0 || distance < closestLightDistance;

            closestLightDistance = distance;
            closestLightStruct = _lights[i];
        }

        if (closestLightStruct.Obj == null)
            return valid = true;

        return valid = FireRay(closestLightStruct.Obj.transform.position, objPos, closestLightStruct,
            closestLightDistance, obj);
    }
    private bool FireRay(Vector3 origin, Vector3 target, LightStruct light, float distance, GameObject obj) //fires a raycast.
    {
        var lightValue = 0f;
        var objMask = LayerMask.LayerToName(obj.layer);
        var mask = LayerMask.GetMask(objMask) | LayerMask.GetMask("Environment");
        var direction = target - origin;
        if (Physics.Raycast(origin, direction, out var hit, light.Range, mask))
        {
            var objTag = hit.transform.tag;
            Debug.DrawLine(origin, hit.point);
            lightValue = objTag == obj.tag ? CalculateLightValue(distance, light.Range, light.Intensity) : 0f;
        }
        return lightValue <= 25;
    }

    public void SetList() //Used to set a list of characters. Called in the Game Manager when respawning characters.
    {
        var lightables = FindObjectsOfType<MonoBehaviour>().OfType<ILightable>();


        //Adds all characters in game to a list of CharacterStructs
        foreach (var obj in lightables)
        {
            var lightable = obj as MonoBehaviour;
            if (lightable == null) continue;
            var newChar = new CharacterStruct();
            _characters.Add(InitCharacterStruct(lightable, newChar));
        }
    }
    
    private void OnDrawGizmos()
    {
        /*
        for (var i = 0; i < _lightGameObjects.Length; i++)
        {
            Gizmos.color = Color.yellow * 0.5f;
            Gizmos.DrawWireSphere(_lightGameObjects[i].transform.position, _lightGameObjects[i].range);
        }
        */
    }
    
}