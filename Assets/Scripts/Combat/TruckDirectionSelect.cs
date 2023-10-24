using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;
using UnityEngine.Events;

public class TruckDirectionSelect : MonoBehaviour, IResetable {
    [SerializeField] Collider2D[] _boarders = null;

    private void Awake() {
        Reset();
    }

    public void Reset() {
        for (int i = 0; i < _boarders.Length; ++i) {
            _boarders[i].gameObject.SetActive(true);
        }
    }

    public Vector2 GetStartPoint(Truck truckObject, Vector2 start, Vector2 end) {
        var hit = Physics2D.Raycast(start, (start - end), 1000f, 1 << LayerMask.NameToLayer("Boarder"));
        Vector2 startPoint = hit.point;
        Vector2 direction = (end - start).normalized;
        startPoint = startPoint - direction * truckObject.Width;
        
        return startPoint;
    }

    public (Vector2, Vector2, float) GetTruckMoveConfig(Truck truckObject, Vector2 startPosition, Vector2 endPosition) {
        float degree = ExVector.GetDegree(startPosition, endPosition);
        Vector2 direction = (endPosition - startPosition).normalized;

        Collider2D[] overlapedBoarders = Physics2D.OverlapCircleAll(startPosition, truckObject.Width);

        foreach (Collider2D boarder in _boarders) {
            foreach (Collider2D overlapedBoarder in overlapedBoarders) {
                if (boarder.Equals(overlapedBoarder)) {
                    boarder.gameObject.SetActive(false);
                    break;
                }
            }
        }

        return (startPosition, direction, degree);
    }
}