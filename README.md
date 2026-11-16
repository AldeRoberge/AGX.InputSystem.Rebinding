<!-- lang -->**Français** | [English](README.en.md)

# AGX.InputSystem.Rebinding

## Aperçu

AGX.InputSystem.Rebinding est une solution facile à utiliser pour le remappage dynamique des contrôles aux actions d'entrée dans Unity avec le nouveau Input System. Fonctionne de façon transparente avec le code généré en C#. Prend en charge d'emblée les manettes Xbox, le clavier et la souris.

![AGX Input System Rebinding](https://github.com/user-attachments/assets/580645fe-42a9-4ac5-a3d6-07dfc19806f4)

## Fonctionnalités

- **Associations d'entrées personnalisées** : Remappez facilement les actions d'entrée, même avec du code généré en C#.
- **Prise en charge des périphériques** : Prise en charge d'emblée de la manette Xbox, du clavier et de la souris. Il est facile d'ajouter d'autres périphériques.
- **Associations composites** : Prend en charge les associations composites comme WASD ou des regroupements personnalisés.
- **Paramètres persistants** : Enregistre et charge automatiquement les mappages d'entrée définis par l'utilisateur, ce qui assure la cohérence d'une session à l'autre.
- **Interface adaptative** : L'interface s'adapte à différentes tailles d'écran et résolutions, ce qui la rend parfaite tant pour les plateformes de bureau que mobiles.
- **Fonction de recherche** : Trouvez rapidement des actions ou des associations grâce à la fonction de recherche intégrée.

https://github.com/user-attachments/assets/5eb7741f-b9a8-49d5-810d-d8158b092efc

## Crédits

- **One Wheel Studio** : Basé sur le travail de [One Wheel Studio](https://www.youtube.com/watch?v=TD0R5x0yL0Y).
- **ChrisKirby14** : Merci à [@ChrisKirby14](https://github.com/ChrisKirby14/InputSystemRebinding)
- **simonoliver** : Inspiré de [InputSystemActionPrompts](https://github.com/simonoliver/InputSystemActionPrompts).
- **Kenney** : Invites d'entrée créées par [Kenney](https://kenney.nl/assets/input-prompts).

## Feuille de route (À FAIRE)

- **Prévention des touches en double** : Ajouter une fonctionnalité pour empêcher les associations de touches en double, y compris des avertissements ou des demandes de confirmation.
- **Blocage de touches spécifiques** : Option pour empêcher le remappage de certaines touches.
- **Prise en charge des touches de modification** : Permettre d'utiliser des modificateurs comme SHIFT, CTRL, etc., lors du remappage.
- **Génération dynamique de l'interface** : Analyser automatiquement les actions d'entrée et construire l'interface, afin de réduire le travail manuel de placement des prefabs.
- **Recherche améliorée** : Améliorer la fonction de recherche pour permettre aux utilisateurs de trouver des actions en détectant les touches pressées (p. ex. : appuyez sur « E » pour mettre en évidence l'action associée à « E »).

## Licence

Ce projet est libre (open source) et distribué sous la [licence MIT](LICENSE).

## Avis de non-responsabilité

Ceci est un prototype rapide conçu à des fins d'expérimentation.
Utilise [TexturePacker](https://www.codeandweb.com/texturepacker/documentation/user-interface-overview) pour assembler les sprites des invites.

## Ressources tierces

Ce projet utilise trois excellentes dépendances tierces payantes.

- DOTween Pro
- Odin Serializer
- Odin Validator

Vous devrez les télécharger manuellement depuis l'Asset Store, car elles ne sont pas incluses par défaut.

Si vous ne souhaitez pas payer ou si vous voulez un projet entièrement libre (OSS), vous pouvez remplacer :
DOTween Pro par une bibliothèque d'animation (tweening) de votre choix.
Odin par Naughty Attributes.


# Ce projet nécessite DOTween Pro, qui n'est pas inclus parce qu'il s'agit d'un logiciel tiers payant.  
Vous pouvez facilement vous en passer ou le remplacer par une autre bibliothèque d'animation. Je l'ai utilisé parce que c'est la solution la plus simple que je connaisse, mais il est tout à fait facultatif.  

![image](https://github.com/user-attachments/assets/5a8f95e5-37d0-4251-8eb0-3ca79284ea7f)
