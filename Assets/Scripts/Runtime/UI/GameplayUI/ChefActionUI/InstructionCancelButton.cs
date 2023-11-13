using Runtime.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class InstructionCancelButton : MonoBehaviour
    {
        [SerializeField] 
        private Button _cancelButton;
        
        [SerializeField] 
        private Image _instructionIngredientImage;

        [SerializeField] 
        private Image _instructionActionImage;
        
        private Instruction _chefInstruction;
        
        public void RemoveInstruction()
        {
            _chefInstruction = null;
            _cancelButton.gameObject.SetActive(false);
        }

        public void SetInstruction(Instruction _instruction)
        {
            _chefInstruction = _instruction;
            _instructionIngredientImage.sprite = _chefInstruction.Ingredient.IngredientIcon;
            _instructionActionImage.sprite = _chefInstruction.StationAction.StationIcon;
            _cancelButton.gameObject.SetActive(true);
        }

        public void OnCancelInstructionButtonClicked()
        {
            _chefInstruction.Chef.CancelInstruction(_chefInstruction);
        }
    }
}