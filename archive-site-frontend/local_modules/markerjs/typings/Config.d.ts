import { MarkerAreaState } from './MarkerAreaState';
export default interface Config {
    targetRoot?: HTMLElement;
    renderAtNaturalSize?: boolean;
    markerColors?: MarkerColors;
    strokeWidth?: number;
    renderImageType?: string;
    renderImageQuality?: number;
    renderMarkersOnly?: boolean;
    previousState?: MarkerAreaState;
    showUi: boolean;
}
export interface MarkerColors {
    mainColor: string;
    highlightColor: string;
    coverColor: string;
}
