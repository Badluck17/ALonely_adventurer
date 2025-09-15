using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Zycalipse.Systems
{
    public class SpineMeshFilter : MonoBehaviour
    {
        // idea arr of string of each slot and the spine handler
        public SkeletonAnimation Animation;
        private List<Attachment> SpineHandler;
    }
}
