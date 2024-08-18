import { Vector3 } from "./drawing.entity";

export class DrawingDto {
    linesPoints: Vector3[][];
    linesWidth: number[];
    linesColorIndex: number[];
    userName: string;
    drawingName: string;
    background: string;
    score: number;
}