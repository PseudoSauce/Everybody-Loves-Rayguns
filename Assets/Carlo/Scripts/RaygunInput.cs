using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaygunComponent))]
public class RaygunInput : MonoBehaviour
{
    #region Variables
    private GunMode m_currentGunMode = GunMode.Scaler;
    private RaygunComponent m_raygun = null;

    // Axis names //
    [SerializeField]
    private string m_primaryFire;
    [SerializeField]
    private string m_secondaryFire;
    [SerializeField]
    private string m_swapWeaponRight;
    [SerializeField]
    private string m_swapWeaponLeft;
    [SerializeField]
    private string m_toggleWeapon;
    [SerializeField]
    private string m_rotateX;
    [SerializeField]
    private string m_rotateY;
    #endregion Variables

    #region Monobehaviour
    void Start()
    {
        m_raygun = GetComponent<RaygunComponent>();
    }

    void Update()
    {
        if(m_raygun != null)
        {
            // Input
            ProcessInput();

            // Passive functionality
            switch (m_currentGunMode)
            {
                case GunMode.Teleporter:
                    m_raygun.DisplayHologram();
                    break;
                case GunMode.Scaler:

                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_raygun != null)
        {
            if (other.CompareTag("ForceField"))
            {
                m_raygun.DestroyBeacon();
            }
        }
    }
    #endregion Monobehaviour

    #region Raygun
    private void ProcessInput()
    {
        // Process the input
        if (Input.GetButtonDown(m_swapWeaponLeft))
        {
            // Swap weapon
            ChangeGunMode(false);
            // Stop last weapon
            if (m_currentGunMode != GunMode.Scaler)
                m_raygun.StopScaling();
        }
        else if (Input.GetButtonDown(m_swapWeaponRight))
        {
            // Swap weapon
            ChangeGunMode(true);
            // Stop last weapon
            if (m_currentGunMode != GunMode.Scaler)
                m_raygun.StopScaling();
        }
        else if(Input.GetButtonDown(m_toggleWeapon))
        {
            // Swap weapon
            ChangeGunMode(false);
            // Stop last weapon
            if (m_currentGunMode != GunMode.Scaler)
                m_raygun.StopScaling();
        }

        if (Input.GetButtonDown(m_primaryFire))
        {
            // Shoot primary fire
            switch (m_currentGunMode)
            {
                case GunMode.Teleporter:
                    m_raygun.TeleportBeam();
                    break;
                case GunMode.Scaler:
                    m_raygun.ShootRay("growing");
                    break;
            }
        }
        else if (Input.GetButtonDown(m_secondaryFire))
        {
            // Shoot secondary fire
            switch (m_currentGunMode)
            {
                case GunMode.Teleporter:
                    m_raygun.DeployBeacon();
                    break;
                case GunMode.Scaler:
                    m_raygun.ShootRay("shrinking");
                    break;
            }
        }

        if (Input.GetButtonDown(m_rotateX))
        {
            // Check for teleport beam and then rotate
            if (m_currentGunMode == GunMode.Teleporter)
            {
                m_raygun.RotateHologram(true);
            }
        }
        else if (Input.GetButtonDown(m_rotateY))
        {
            // Check for teleport beam and then rotate
            if (m_currentGunMode == GunMode.Teleporter)
            {
                m_raygun.RotateHologram(false);
            }
        }

        if (Input.GetButtonUp(m_primaryFire))
        {
            // Stop firing
            switch (m_currentGunMode)
            {
                case GunMode.Teleporter:

                    break;
                case GunMode.Scaler:
                    m_raygun.StopScaling();
                    break;
            }
        }
        else if (Input.GetButtonUp(m_secondaryFire))
        {
            // Stop firing
            switch (m_currentGunMode)
            {
                case GunMode.Teleporter:

                    break;
                case GunMode.Scaler:
                    m_raygun.StopScaling();
                    break;
            }
        }
    }

    private void ChangeGunMode(bool right)
    {
        if (right)
        {
            m_currentGunMode++;

            if (m_currentGunMode >= GunMode.ModeCount)
            {
                m_currentGunMode = 0;
            }
        }
        else 
        {
            m_currentGunMode--;

            if (m_currentGunMode < 0)
            {
                m_currentGunMode = GunMode.ModeCount - 1;
            }
        }
    }
    #endregion Raygun
}
