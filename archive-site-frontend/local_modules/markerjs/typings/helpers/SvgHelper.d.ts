export declare class SvgHelper {
    static createRect: (width: string | number, height: string | number, attributes?: [string, string][]) => SVGRectElement;
    static createLine: (x1: string | number, y1: string | number, x2: string | number, y2: string | number, attributes?: [string, string][]) => SVGLineElement;
    static createPolygon: (points: string, attributes?: [string, string][]) => SVGPolygonElement;
    static createCircle: (radius: number, attributes?: [string, string][]) => SVGCircleElement;
    static createEllipse: (rx: number, ry: number, attributes?: [string, string][]) => SVGEllipseElement;
    static createGroup: (attributes?: [string, string][]) => SVGGElement;
    static setAttributes: (el: SVGElement, attributes: [string, string][]) => void;
    static createTransform: () => SVGTransform;
    static createDefs: () => SVGDefsElement;
    static createMarker: (id: string, orient: string, markerWidth: string | number, markerHeight: string | number, refX: string | number, refY: string | number, markerElement: SVGGraphicsElement) => SVGMarkerElement;
    static createText: (attributes?: [string, string][]) => SVGTextElement;
    static createTSpan: (text: string, attributes?: [string, string][]) => SVGTSpanElement;
}
