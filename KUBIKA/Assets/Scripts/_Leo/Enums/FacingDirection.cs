using UnityEngine;

namespace Kubika.Game
{
    public class CubeFacingDirection : MonoBehaviour
    {
        public static Vector3 CubeFacing(FacingDirection facingDirection)
        {
            Vector3 vectorResult = Vector3.up;

            switch (facingDirection)
            {
                case FacingDirection.up:
                    vectorResult = new Vector3(270, 0, 0);
                    break;
                case FacingDirection.down:
                    vectorResult = new Vector3(90, 0, 0);
                    break;
                case FacingDirection.forward:
                    vectorResult = new Vector3(0, 0, 0);
                    break;
                case FacingDirection.backward:
                    vectorResult = new Vector3(180, 0, 0);
                    break;
                case FacingDirection.right:
                    vectorResult = new Vector3(0, 90, 0);
                    break;
                case FacingDirection.left:
                    vectorResult = new Vector3(0, 270, 0);
                    break;

                default:
                    break;
            }

            Debug.Log(vectorResult);
            return vectorResult;
        }

        public static FacingDirection CubeFacingFromRotation(Vector3 localEulerRotation)
        {

            if(localEulerRotation == new Vector3(270, 0, 0))
            {
                return FacingDirection.up;
            }
            else if (localEulerRotation == new Vector3(90, 0, 0))
            {
                return FacingDirection.down;
            }
            else if (localEulerRotation == new Vector3(0, 0, 0))
            {
                return FacingDirection.forward;
            }
            else if (localEulerRotation == new Vector3(180, 0, 0))
            {
                return FacingDirection.backward;
            }
            else if (localEulerRotation == new Vector3(0, 90, 0))
            {
                return FacingDirection.right;
            }
            else if (localEulerRotation == new Vector3(0, 270, 0))
            {
                return FacingDirection.left;
            }
            else
            {
                Debug.LogError("FACING_DIRECTION_PROBLEM");
                return FacingDirection.forward;
            }

        }
    }
}

public enum FacingDirection
{
    up,
    down,

    forward,
    backward,

    right,
    left,
}