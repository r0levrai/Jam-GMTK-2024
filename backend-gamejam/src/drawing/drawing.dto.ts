import { Points, Vector3 } from "./drawing.entity";

export class DrawingDto {
    linesPoints: Points[];
    linesWidth: number[];
    linesColorIndex: number[];
    userName: string;
    drawingName: string;
    background: string;
    score: number;
}