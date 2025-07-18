// SPDX-FileCopyrightText: 2020 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Arcade
{
    [Serializable, NetSerializable]
    public enum BlockGamePlayerAction
    {
        NewGame,
        StartLeft,
        EndLeft,
        StartRight,
        EndRight,
        Rotate,
        CounterRotate,
        SoftdropStart,
        SoftdropEnd,
        Harddrop,
        Pause,
        Unpause,
        Hold,
        ShowHighscores
    }
}
