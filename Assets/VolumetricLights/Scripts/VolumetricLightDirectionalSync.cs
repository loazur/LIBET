using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricLights {
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    [RequireComponent(typeof(VolumetricLight))]
    public class VolumetricLightDirectionalSync : MonoBehaviour {

        [Tooltip("The directional light that is synced with this volumetric area light.")]
        public Light directionalLight;
        [Tooltip("Makes this area light position follow the desired target. Usually this is the main camera.")]
        public Transform follow;
        [Tooltip("Move volumetric light to 'follow' gameobject position if distance is greater than this value. Updating the position of this volumetric light area every frame is not recommended.")]
        public float distanceUpdate = 1f;
        
        [Header("Color Sync")]
        [Tooltip("Synchronize the volumetric light color with the directional light color.")]
        public bool syncColor = true;
        [Tooltip("Multiplier for the synced color saturation (1 = same as sun, <1 = more desaturated).")]
        [Range(0.5f, 2f)]
        public float colorSaturation = 1f;

        VolumetricLight vl;
        Light fakeLight;
        Vector3 lastFollowPos;
        Quaternion lastSunRotation;
        Color lastSunColor;

        private void Start() {
            vl = GetComponent<VolumetricLight>();
            fakeLight = GetComponent<Light>();
            if (follow == null && Camera.main != null) follow = Camera.main.transform;
            if (directionalLight != null) {
                lastSunRotation = directionalLight.transform.rotation;
                lastSunColor = directionalLight.color;
            }
        }


        void LateUpdate() {
            if (directionalLight != null) {
                bool needsUpdate = false;
                
                // Vérifier si le soleil a tourné
                if (Quaternion.Angle(lastSunRotation, directionalLight.transform.rotation) > 0.1f) {
                    lastSunRotation = directionalLight.transform.rotation;
                    needsUpdate = true;
                }
                
                // Vérifier si le joueur/caméra a bougé
                if (follow != null) {
                    Vector3 followPos = follow.position;
                    if (Vector3.Distance(lastFollowPos, followPos) > distanceUpdate) {
                        lastFollowPos = followPos;
                        needsUpdate = true;
                    }
                }
                
                // Mettre à jour la position si nécessaire
                if (needsUpdate && follow != null) {
                    transform.position = follow.position;
                    transform.position -= directionalLight.transform.forward * vl.generatedRange * 0.5f;
                }
                
                transform.forward = directionalLight.transform.forward;
                
                // Synchroniser la couleur avec le soleil
                if (syncColor && vl != null) {
                    Color sunColor = directionalLight.color;
                    // Ajuster la saturation si nécessaire
                    if (colorSaturation != 1f) {
                        float h, s, v;
                        Color.RGBToHSV(sunColor, out h, out s, out v);
                        s = Mathf.Clamp01(s * colorSaturation);
                        sunColor = Color.HSVToRGB(h, s, v);
                    }
                    vl.mediumAlbedo = sunColor;
                }
                
                if (fakeLight != null) {
                    // Ne pas désactiver la lumière, juste synchroniser les propriétés
                    // fakeLight.enabled = false;
                    fakeLight.color = directionalLight.color;
                    fakeLight.intensity = directionalLight.intensity;
                }
            }

        }
    }

}