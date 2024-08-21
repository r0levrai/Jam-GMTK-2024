import { Controller, Get, Post, Put, Delete, Param, Body, Query, Req, BadRequestException } from "@nestjs/common";
import { DrawingService } from "./drawing.service";
import { Drawing } from "./drawing.entity";
import { DrawingDto } from "./drawing.dto";
import { Request } from 'express';
import { DrawingResponseDto } from "./drawing.response.dto";
import { PageOptionsDto } from "src/config/page/page-option.dto";
import { PageDto } from "src/config/page/page.dto";



@Controller('drawings')
export class DrawingController {
    constructor(private readonly drawingService: DrawingService) {}

    @Post()
    async createDrawing(@Body() drawingDto: DrawingDto): Promise<DrawingResponseDto> {
        return await this.drawingService.createDrawing(drawingDto);
    }

    @Get('all')
    async getAllDrawings(@Query() pageOptionsDto: PageOptionsDto): Promise<PageDto<DrawingResponseDto>> {
        try {
            return await this.drawingService.getAllDrawings(pageOptionsDto);
        } catch (error) {
            console.error("Error fetching all drawings:", error);
            throw new BadRequestException("Could not fetch all drawings");
        }
    }

    @Get('ordered-by-likes')
    async getDrawingsOrderedByLikes(@Query() pageOptionsDto: PageOptionsDto): Promise<PageDto<DrawingResponseDto>> {
        try {
            return await this.drawingService.getDrawingsOrderedByLikes(pageOptionsDto);
        } catch (error) {
            console.error("Error fetching drawings ordered by likes:", error);
            throw new BadRequestException("Could not fetch drawings ordered by likes");
        }
    }

    @Get('random')
    async getRandomDrawings(@Query() pageOptionsDto: PageOptionsDto): Promise<PageDto<DrawingResponseDto>> {
        try {
            return await this.drawingService.getRandomDrawings(pageOptionsDto);
        } catch (error) {
            console.error("Error fetching random drawings:", error);
            throw new BadRequestException("Could not fetch random drawings");
        }
    }

    @Get('lasts')
    async getLastDrawings(@Query('n') n: number){
        const drawings = await this.drawingService.getLastDrawings(n);
        return {drawings: drawings};
    }

    @Get('firsts')
    async getFirstDrawings(@Query('n') n: number) {
        const drawings = await this.drawingService.getFirstDrawings(n);
        return {drawings: drawings};
    }

    @Get('user/:userName')
    async getDrawingsByUserName(@Param('userName') userName: string) {
        const drawings = await this.drawingService.getDrawingsByUserName(userName);
        return {drawings: drawings};

    }

    @Put(':id')
    async updateDrawing(@Param('id') id: number, @Body() updateData: Partial<Drawing>): Promise<DrawingResponseDto> {
        return await this.drawingService.updateDrawing(id, updateData);
    }

    @Get("one/:id")
    async getDrawingById(@Param('id') id: number): Promise<DrawingResponseDto> {
        return await this.drawingService.getDrawingById(id);
    }

    @Delete(':id')
    async deleteDrawing(@Param('id') id: number): Promise<void> {
        await this.drawingService.deleteDrawing(id);
    }

    
    @Post(':id/:reaction')
    async addReaction(
        @Param('id') id: number,
        @Param('reaction') reaction: string,
        @Req() request: Request
    ): Promise<DrawingResponseDto> {
        const ipAdress = request.ip;
        return await this.drawingService.addReaction(id, reaction, ipAdress);
    }
}