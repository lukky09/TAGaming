using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoordinateMovement : MonoBehaviour
{
    Coordinate _targetCoordinate;
    Coordinate[] _pathCoordinates;
    int _currentPathIndex;
    BotActions _botActRef;

    private void Start()
    {
        _botActRef = GetComponent<BotActions>();
    }

    public void setTargetCoordinate(Vector2 targetCoordinate)
    {
        setTargetCoordinate(Coordinate.returnAsCoordinate(targetCoordinate));
    }

    public void setTargetCoordinate(Coordinate targetCoordinate)
    {
        
        _targetCoordinate = targetCoordinate;
        if (targetCoordinate.Equal(Coordinate.returnAsCoordinate(transform.position)))
            _pathCoordinates = new Coordinate[] { new Coordinate(targetCoordinate.xCoor, targetCoordinate.yCoor) };
        else
            _pathCoordinates = AStarAlgorithm.makeWay(Coordinate.returnAsCoordinate(transform.position), targetCoordinate);
        _currentPathIndex = 0;
        _botActRef.setWalkLocation(_pathCoordinates[_currentPathIndex]);
    }

    public void setPathCoordinates(Coordinate[] pathCoordinate)
    {
        if (pathCoordinate == null)
            return;
        _pathCoordinates = pathCoordinate;
        _targetCoordinate = pathCoordinate[pathCoordinate.Length - 1];
        _currentPathIndex = 0;
        _botActRef.setWalkLocation(_pathCoordinates[_currentPathIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetCoordinate == null || _currentPathIndex == _pathCoordinates.Length || !GetComponent<SnowBrawler>().canAct)
            return;
        //Debug.Log(_pathCoordinates[_currentPathIndex]);
        if (Vector2.Distance(transform.position, _pathCoordinates[_currentPathIndex].returnAsVector()) < 0.25 && _currentPathIndex< _pathCoordinates.Length-1)
        {
            _currentPathIndex++;
            _botActRef.setWalkLocation(_pathCoordinates[_currentPathIndex]);
        }
    }

    public void stopMoving()
    {
        _targetCoordinate = null;
        _botActRef.setWalkLocation(Vector2.zero);
    }

    public bool hasArrivedtoDestination()
    {
        if (_targetCoordinate == null)
            return true;
        return (Vector2.Distance(_targetCoordinate.returnAsVector(), transform.position) < 0.25);
    }


}
