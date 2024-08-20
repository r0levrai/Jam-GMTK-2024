import { Points } from "./drawing.entity";

export class DrawingResponseDto{
    id: number;

    createdDate: Date;

    linesPoints: Points[];

    linesWidth: number[];

    linesColorIndex: number[];

    userName: string;

    drawingName: string;

    background: string;

    score: number;

    like: number;

    funny: number;

    bad: number;
}