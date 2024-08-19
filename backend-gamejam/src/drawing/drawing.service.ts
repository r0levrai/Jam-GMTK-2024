import { BadRequestException, Injectable, NotFoundException } from "@nestjs/common";
import { InjectRepository } from "@nestjs/typeorm";
import { DrawingRepository } from "./drawing.repository";
import { Drawing } from "./drawing.entity";
import { DrawingDto } from "./drawing.dto";
import { Reaction } from "src/reaction/reaction.entity";
import { ReactionRepository } from "src/reaction/reaction.repository";

@Injectable()
export class DrawingService {
    private readonly MAX_RANDOM_DRAWINGS = 100;

    constructor(
        @InjectRepository(DrawingRepository)
        private readonly drawingRepository: DrawingRepository,
        @InjectRepository(ReactionRepository)
        private readonly reactionRepository: ReactionRepository
    ) {}
    

    async createDrawing(drawingDto: DrawingDto): Promise<Drawing> {
        return await this.drawingRepository.createDrawing(drawingDto);
    }

    async getDrawingById(id: number): Promise<Drawing | undefined> {
        return await this.drawingRepository.findDrawingById(id);
    }

    async getDrawingsByUserName(userName: string): Promise<Drawing[]> {
        return await this.drawingRepository.findByUserName(userName);
    }

    async updateDrawing(id: number, updateData: Partial<DrawingDto>): Promise<Drawing> {
        return await this.drawingRepository.updateDrawing(id, updateData);
    }

    async deleteDrawing(id: number): Promise<void> {
        await this.drawingRepository.deleteDrawing(id);
    }

    async getLastDrawings(limit: number): Promise<Drawing[]> {
        return await this.drawingRepository.findLastDrawings(limit);
    }

    async getFirstDrawings(limit: number): Promise<Drawing[]> {
        return await this.drawingRepository.findFirstDrawings(limit);
    }
    async getRandomDrawings(limit: number): Promise<Drawing[]> {
        const effectiveLimit = Math.min(limit, this.MAX_RANDOM_DRAWINGS);
        return await this.drawingRepository.findRandomDrawings(effectiveLimit);
    }

    async addReaction(drawingId: number, ipAddress: string, reaction: string): Promise<Reaction> {
        const drawing = await this.drawingRepository.findOne({ where: { id: drawingId }, relations: ['reactions'] });
        if (!drawing) {
            throw new NotFoundException("Not Found");
        }

        console.log(this.reactionRepository)
        console.log(this.drawingRepository)
        const existingReaction = await this.reactionRepository.findByDrawingAndIpAddress(drawing, ipAddress)
        if (existingReaction) {
            //if (existingReaction.reaction === reaction)
                throw new BadRequestException('User has already reacted to this drawing');
        }

        const newReaction = new Reaction();
        newReaction.drawing = drawing;
        newReaction.ipAddress = ipAddress;
        newReaction.reaction = reaction;

        return await this.reactionRepository.save(newReaction);
    }
}