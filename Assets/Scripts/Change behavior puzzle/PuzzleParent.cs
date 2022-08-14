using PuzzleGeneration;
using UnityEngine;

public class PuzzleParent
{
    private RectTransform _containerTransform;
    private Transform _puzzleGeneratorTransform;
    private PuzzleGluer _puzzleGluer;

    public PuzzleParent(RectTransform containerTransform, Transform puzzleGenerator, PuzzleGluer puzzleGluer,
        DragHandler[] dragHandlers)
    {
        _containerTransform = containerTransform;
        _puzzleGeneratorTransform = puzzleGenerator;
        _puzzleGluer = puzzleGluer;
        SubscribeOnEvents(dragHandlers);
    }

    //public void OnDestroyGameLogicContoller()
    //{
    //    //Unsubscribe();
    //}

    private void SubscribeOnEvents(DragHandler[] dragHandlers)
    {
        _puzzleGluer.OnChangeMauseOnBoard += (bool OnBoard, PiecePuzzle piecePuzzle) =>
        piecePuzzle.transform.SetParent(OnBoard ? _puzzleGeneratorTransform : _containerTransform);

        foreach (var handler in dragHandlers)
            handler.OnBeginDragging += (DragHandler handler) =>
            handler.transform.SetParent(_puzzleGeneratorTransform);
        // как отписаться от лямда
    }

    //private void Unsubscribe()
    //{
    //    _puzzleGluer.OnChangeMauseOnBoard -= ;
    //}
}
