using UnityEngine;

namespace CustomCentisMod;

public class CustomCentiShell(Vector2 pos, Vector2 vel, float hue, float saturation, float scaleX, float scaleY, string sprite, Color newBlackColor) : CentipedeShell(pos, vel, hue, saturation, scaleX, scaleY)
{
    public readonly string Sprite = sprite;
    public readonly Color NewBlackColor = newBlackColor;

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        base.InitiateSprites(sLeaser, rCam);
        var s = TryGetSprite(Sprite, "CentipedeBackShell");
        sLeaser.sprites[0].element = s;
        sLeaser.sprites[1].element = s;
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);
        blackColor = NewBlackColor;
    }
}