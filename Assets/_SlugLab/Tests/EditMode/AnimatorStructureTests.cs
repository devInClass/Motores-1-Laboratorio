using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-40: el Animator del jugador debe usar blend trees, no estados sueltos.
    /// </summary>
    [TestFixture]
    public class AnimatorStructureTests
    {
        private const string ControllerPath = "Assets/_SlugLab/Settings/Datos/Animaciones/Player.controller";

        [Test]
        public void TC40_ElAnimatorDelJugadorUsaDosBlendTrees()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            Assert.IsNotNull(controller, "Falta el AnimatorController del jugador.");

            AnimatorState[] states = controller.layers[0].stateMachine.states.Select(s => s.state).ToArray();
            AnimatorState locomotion = states.FirstOrDefault(s => s.name == "Locomotion");
            AnimatorState aim = states.FirstOrDefault(s => s.name == "Aim");

            Assert.IsNotNull(locomotion, "Falta el estado Locomotion.");
            Assert.IsNotNull(aim, "Falta el estado Aim.");

            var locomotionTree = locomotion.motion as BlendTree;
            Assert.IsNotNull(locomotionTree, "Locomotion debe ser un blend tree.");
            Assert.AreEqual(BlendTreeType.Simple1D, locomotionTree.blendType);
            Assert.AreEqual("Speed01", locomotionTree.blendParameter);

            var aimTree = aim.motion as BlendTree;
            Assert.IsNotNull(aimTree, "Aim debe ser un blend tree 2D.");
            Assert.AreEqual(BlendTreeType.FreeformCartesian2D, aimTree.blendType);
            Assert.AreEqual("AimAngle", aimTree.blendParameter);
            Assert.AreEqual("Speed01", aimTree.blendParameterY);
            Assert.AreEqual(10, aimTree.children.Length, "5 direcciones x (de pie, corriendo).");

            Assert.IsTrue(aimTree.children.Any(c => Mathf.Approximately(c.position.x, 90f)), "Falta apuntar arriba.");
            Assert.IsTrue(aimTree.children.Any(c => Mathf.Approximately(c.position.x, 0f)), "Falta apuntar al frente.");
            Assert.IsTrue(aimTree.children.Any(c => Mathf.Approximately(c.position.x, -90f)), "Falta apuntar abajo.");
        }

        [Test]
        public void TC40b_ExistenLosParametrosQueUsaElCodigo()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            Assert.IsNotNull(controller);

            string[] required = { "Speed01", "AimAngle", "Aiming", "Grounded", "Crouch", "Dead", "Hurt", "Melee" };
            foreach (string parameter in required)
            {
                Assert.IsTrue(controller.parameters.Any(p => p.name == parameter), $"Falta el parámetro '{parameter}'.");
            }
        }
    }
}
