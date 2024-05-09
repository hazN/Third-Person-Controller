using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace TPC.Combat
{
    public class Targeter : MonoBehaviour
    {
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float targetRadius = 2f;
        private List<Target> targets = new List<Target>();
        private List<Target> sortedTargets = new List<Target>();
        private Camera camera;
        public Target CurrentTarget { get; private set; }
        private void Awake()
        {
            camera = Camera.main;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Target target))
            {
                targets.Add(target);
                target.OnDestroyed += RemoveTarget;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Target target))
            {
                RemoveTarget(target);
            }
        }
        public bool SelectTarget()
        {
            if (targets.Count == 0)
                return false;

            Vector3 cameraForward = camera.transform.forward;
            Vector3 cameraPosition = camera.transform.position;

            sortedTargets = targets.Where(x => Vector3.Dot(cameraForward, x.transform.position - cameraPosition) > 0).ToList();

            if (CurrentTarget == null)
            {
                CurrentTarget = sortedTargets[sortedTargets.Count / 2];
                targetGroup.AddMember(CurrentTarget.transform, 1, targetRadius);
                return true;
            }
            else
            {
                int direction = inputReader.LookValue.x > 0 ? -1 : 1;

                // Make sure the index is within the bounds of the list
                if (sortedTargets.IndexOf(CurrentTarget) + direction >= 0 && sortedTargets.IndexOf(CurrentTarget) + direction < sortedTargets.Count)
                {
                    targetGroup.RemoveMember(CurrentTarget.transform);
                    CurrentTarget = sortedTargets[sortedTargets.IndexOf(CurrentTarget) + direction];
                    targetGroup.AddMember(CurrentTarget.transform, 1, targetRadius);
                    return true;
                }
                else
                {
                    targetGroup.RemoveMember(CurrentTarget.transform);
                    CurrentTarget = null;
                    return false;
                }
            }
        }

        public void Cancel()
        {
            if (CurrentTarget == null) return;
            targetGroup.RemoveMember(CurrentTarget.transform);
            CurrentTarget = null;
        }
        private void RemoveTarget(Target target)
        {
            if (CurrentTarget == target)
            {
                targetGroup.RemoveMember(CurrentTarget.transform);
                CurrentTarget = null;
            }

            target.OnDestroyed -= RemoveTarget;
            targets.Remove(target);
        }
        public bool HasTarget()
        {
            return CurrentTarget != null;
        }
    }
}