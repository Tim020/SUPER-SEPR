using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Human player.
/// </summary>
public class HumanPlayer : Player
{

	public override void OnStartClient ()
	{
		base.OnStartClient ();
	}

	public override void OnStartLocalPlayer ()
	{
		CmdSetCameraTransform ();
	}

	[Command]
	private void CmdSetCameraTransform ()
	{
		Camera.main.transform.position = new Vector3 (MapController.instance.width / 2, MapController.instance.height / 2, -30);
	}
}
