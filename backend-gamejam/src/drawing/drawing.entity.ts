import { BaseEntity, Column, CreateDateColumn, Entity, PrimaryGeneratedColumn, ValueTransformer } from "typeorm";



export class Vector3 {
    x: number;
    y: number;
    z: number;
}

export class Points {
    points: Vector3[];
}

export enum ReactionType {
    LIKE = "like",
    FUNNY = "funny",
    BAD = "bad",
}

const mapTransformer: ValueTransformer = {
    to: (value: Map<string, number>) => JSON.stringify(Array.from(value.entries())),
    from: (value: string) => new Map<string, number>(JSON.parse(value))
};

@Entity()
export class Drawing extends BaseEntity {
    @PrimaryGeneratedColumn()
    id: number;

    @CreateDateColumn()
    createdDate: Date;

    @Column("json")
    linesPoints: Points[];

    @Column("simple-array")
    linesWidth: number[];

    @Column("simple-array")
    linesColorIndex: number[];

    @Column()
    userName: string;

    @Column()
    drawingName: string;

    @Column()
    background: string;

    @Column()
    score: number;

    @Column("simple-array")
    like: string[];

    @Column("simple-array")
    funny: string[];

    @Column("simple-array")
    bad: string[];
}