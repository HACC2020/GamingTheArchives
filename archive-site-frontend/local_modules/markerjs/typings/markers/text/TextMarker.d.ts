import { RectangularMarkerBase } from "../RectangularMarkerBase";
import { TextMarkerState } from './TextMarkerState';
export declare class TextMarker extends RectangularMarkerBase {
    static createMarker: () => TextMarker;
    constructor();
    protected readonly MIN_SIZE = 50;
    private readonly DEFAULT_TEXT;
    private text;
    private textElement;
    private inDoubleTap;
    private editor;
    private editorTextArea;
    getState(): TextMarkerState;
    restoreState(state: TextMarkerState): void;
    protected setup(): void;
    protected resize(x: number, y: number): void;
    private renderText;
    private sizeText;
    private onDblClick;
    private onTap;
    private showEditor;
    private onEditorOkClick;
    private closeEditor;
    private onEditorKeyDown;
}
