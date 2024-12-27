# dialogue-system
Built in Unity 2022.3.43f1.

This is a dialogue system that supports 2 way conversations in the screen space UI. It also supports monologing / one way dialogue.

## Features
### Addressables
Scriptable Objects are set as Addressables and are passed into the **DialogueManager**.
The **DialogueManager** discards the asset reference when the dialogue is completed.

### Custom Markup
Try using these custom markup effects:
* \<show> - Shows the dialogue container after being hidden.
* \<hide> - Hides the dialogue container until Show is explicity called again.
* \<shake> - Shakes the dialogue container.
* \<wait=x> - Waits x amount of seconds before resuming dialogue.
* \<speed=x> - Overrides the speed at which the text appears, in milliseconds. Lower is faster.
* \<emotion=x> Sets the portrait of the character to x emotion and plays accompanying sound.

Text Mesh Pro markups can also be used.

### Tasks and Async
UniTask is leveraged for the effect features. No coroutines are used.

## Dependencies
Third party plugins:
* UniTask
* DOTween

## How To Use
### Setup Dialogue Manager
The **DialogueManager** is a standalone class so you have options to make it into:
* A Singleton
* A Monobehaviour on a GameObject
* A Service in a dependency injection workflow.

If you refer to the test scene, there is a mock GameObject that is holding the asset references to the **DialogueScriptableObjects** and it creates the **DialogueManager**.

### Creating a Dialogue
Create a folder to hold your **DialogueScriptableObjects**.
> Right click the folder > Create > Dialogue System > Dialogue

Add an element to **DialogueBlocks** in the Inspector. Configure as you like.

Dialogues require **DialogueCharacters**.
> Right click the folder > Create > Dialogue System > DialogueCharacter

The system expects that you supply 5 emotions for each character:
* Default
* Happy
* Sad
* Angry
* Thinking

There are greybox assets to fill the requirements. You can add or take away emotions in the codebase.

### Triggering Dialogue
Create a script that holds a **DialogueScriptableObjectAssetReference**. See **TriggerButton** for an example and see its use in the test scene.

Depending on your system architecture, get the **DialogueManager**, call **OpenDialogue**() and pass in the asset reference. The manager will do the rest.
