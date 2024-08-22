The main repo for our game 🍌 [Cavendish's Perfect Proportions](https://r0levrai.itch.io/cavendishs-perfect-proportions) 🍌, made in 4 days for the [GMTK 2024 Game Jam](https://itch.io/jam/gmtk-2024)!

Rating and comments from the jam are [here](https://itch.io/jam/gmtk-2024/rate/2901708).

This was made with a 4 days time limit, so expect some messy stuff xD

## Features

**Zoom** from the galaxy to the blood vessels, and **draw** the object to the correct size! Your drawing will be seen by **other players**, and Cavendishs will rate their **resemblance** as well with a totally legit artistic value score!

## Tech

- Backend in NestJs and TypeScript backed by MariaDB, self-hosted on a raspberry pi 5.
- Frontend in Unity and C#, exported as WebGL builds, using the new UI & input system, LineRenderers for the drawing, its points bounding box for size recognition, and a 44Mb neural sketch recogniser (Resnet18) finetuned on part of [those](https://quickdraw.withgoogle.com/data/) [datasets](https://cybertron.cg.tu-berlin.de/eitz/projects/classifysketch/) during the jam for the artistic score value and the 🍌 roast generation 🍌.
- Art drawn in Photoshop and Clip Studio, music and sound composed in FL Studio.
- Repo initially self-hosted on gitea in case we needed more than 10Go of storage or files larger than 2Go.

## Authors (in alphabetical order)

- Art: [blueswanson](https://blueswanson.tumblr.com/), [Hopeful Undead](https://hopeful-undead.itch.io/)
- Music & Sounds: [Osun](https://osun.itch.io/)
- Prog: [Ankos](https://ansoko.itch.io/), [MiniWall](https://miniwall.itch.io/), [misterdoug](https://linktr.ee/ddkhaled), [r0levrai](https://r0levrai.itch.io/), [SuperKuku](https://superkuku.itch.io/)

## License

Code is under the [MIT License](https://opensource.org/license/mit) (~= do what you want).

Art, music and sounds are under [CC-BY](https://creativecommons.org/licenses/by/4.0/) (~= do what you want but credit us!).

Real-world object images that we scale against your drawing in [`dev_unity/Assets/Sprites/RealObjects`](./dev_unity/Assets/Sprites/RealObjects) may need [their own attributions](./objects_attributions).

If this made your day, consider dropping us a message on itch or gitea so we know if open sourcing is worthwhile for future jams and projects!