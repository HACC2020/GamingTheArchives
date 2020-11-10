import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { TextMarker } from "./TextMarker";
export declare class TextMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof TextMarker;
}
